using AppReservation.Data;
using AppReservation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppReservation.ViewModels;
using NToastNotify;

namespace AppReservation.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IToastNotification _toastNotification;

        public ReservationController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IToastNotification toastNotification)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _toastNotification = toastNotification;
        }

        public IActionResult Idx()
        {

            var list = _context.Reservations.Include(s => s.Student).Include(rt => rt.Reserv)
                
                .OrderBy(c => c.Student.ResCount);
            ViewBag.role = new IdentityRole();
            return View(list.ToList().Where(d => d.Date >= DateTime.Today||d.Date.DayOfWeek == DayOfWeek.Saturday || d.Date.DayOfWeek == DayOfWeek.Sunday));
                
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Student"))
                {
                    await GetDataByUser();
                }
                else if (User.IsInRole("Admin"))
                {
                    Idx();
                }
                else
                {
                    return NotFound();
                }
            }
            return View();

        }

        public async Task<IActionResult> GetDataByUser()
        {
            var student = await _userManager.GetUserAsync(HttpContext.User);
            var list = _context.Reservations.Include(s => s.Student).Include(rt => rt.Reserv).Where(s => s.StudentId == student.Id);
            return View("Index", list.ToList());
        }

        // GET: ReservationController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReservationController/Create
        public ActionResult Create()
        {
            var list = _context.TypeReservations;
            ViewBag.types = list.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {

            if (ModelState.IsValid)
            {
                var type = _context.TypeReservations.Where(r => r.Id == reservation.ReservId).FirstOrDefault();
                var student = await _userManager.GetUserAsync(HttpContext.User);

                var reser = new Reservation();
                reser.Status = reservation.Status;
                reser.Date = reservation.Date;
                reser.Cause = reservation.Cause;
                reser.StudentId = student.Id;
                reser.ReservId = type.Id;


                _context.Add(reser);

                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }

            return View(reservation);
        }

        
        public ActionResult Edit(int? id)
        {
            var getid = _context.Reservations.Find(id);
            ViewBag.gettype = _context.TypeReservations.ToList();
            return View(getid);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Reservation reservation)
        {

            if (ModelState.IsValid)
            {
                var type = _context.TypeReservations.Where(r => r.Id == reservation.ReservId).FirstOrDefault();
                //reservation.ReservationType.Id = type.ToString();
                var student = await _userManager.GetUserAsync(HttpContext.User);
                //var studentId = student.Id;


                reservation.StudentId = student.Id;
                reservation.ReservId = type.Id;

                _context.Update(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }

            return View(reservation);
        }

        public IActionResult Delete(int? id)
        {
            var list = _context.Reservations.Include(s => s.Student).Include(rt => rt.Reserv);
            ViewBag.data = list.AsEnumerable();
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var del = _context.Reservations.Find(id);
            return View(del);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {

            var del = _context.Reservations.Find(id);
            _context.Reservations.Remove(del);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public void Increment(int id)
        {
            var usr = _context.Reservations.Find(id);
            //var res = new Student();
            //var inc = usr.Student.resCount;
            //var incr = new Student().resCount;
            
            //var student = await _userManager.GetUserAsync(HttpContext.User);
            var u = _context.Students.FirstOrDefault(s => s.Id == usr.StudentId);
            var inc = usr.Student.ResCount;
            u.ResCount = inc + 1;
            //int inc = Convert.ToInt32(usr.Student.resCount.ToString());
            //res.resCount += inc;
            //usr.Student.resCount = incr + 1;
            _context.Update(usr);
            _context.Update(u);
             _context.SaveChanges();
        }

        public async Task<IActionResult> Confirm(int id)
        {
            var resr = _context.Reservations.Find(id);
            if(resr.Status != "Approved")
            {
                Increment(id);
                //var app = new Reservation();
                resr.Status = "Approved";
                _context.Update(resr);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Reservation approved");
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Reservation already approved");
            }
            
            return RedirectToAction("index");
        }

        public IActionResult Decline(int id)
        {
            var resr = _context.Reservations.Find(id);

            if (resr.Status != "Declined")
            {
                //var app = new Reservation();
                resr.Status = "Declined";
                _context.Update(resr);
                _context.SaveChanges();
                _toastNotification.AddWarningToastMessage("Reservation declined");
                
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Reservation already declined");
            }

            return RedirectToAction("index");

        }
    }
}
