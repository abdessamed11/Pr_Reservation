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

        public IActionResult Resby_res()
        {
            var toutlist = _context.Reservations.Include(s => s.Student).Include(tr => tr.Reserv).OrderBy(s => s.Student.ResCount).ToList();
            ViewBag.role = new IdentityRole();
            return View(toutlist.Where(d => d.Date >= DateTime.Today||d.Date.DayOfWeek == DayOfWeek.Saturday || d.Date.DayOfWeek == DayOfWeek.Sunday));
                
        }

        public IActionResult Res_Approved()
        {

            var list = _context.Reservations.Include(s => s.Student).Include(rt => rt.Reserv);

            ViewBag.role = new IdentityRole();
            return View(list.ToList().Where(d => d.Status == "Approved" && d.Date >= DateTime.Today));

        }

        public IActionResult Res_Declined()
        {

            var list = _context.Reservations.Include(s => s.Student).Include(rt => rt.Reserv);

            ViewBag.role = new IdentityRole();
            return View(list.ToList().Where(d => d.Status == "Declined" && d.Date >= DateTime.Today));

        }

        public ActionResult Filtre()
        {
            var student = _userManager.GetUserId(HttpContext.User);
            var toutlist = _context.Reservations.Include(s => s.Student).Include(tr => tr.Reserv).OrderBy(s => s.Student.ResCount).ToList();
            return View(toutlist.Where(d => d.Date >= DateTime.Today));
            
        }

        public IActionResult Pend()
        {

            var list = _context.Reservations.Include(s => s.Student).Include(rt => rt.Reserv);

            ViewBag.role = new IdentityRole();
            return View(list.ToList().Where(d => d.Status == "pending" && d.Date >= DateTime.Today));

        }

        [HttpPost]
        public ActionResult Index(DateTime? dates)
        {
            var dateres = _context.Reservations
                .Include(s => s.Student)
                .Include(rt => rt.Reserv)
              .Where(t => t.Date == dates);
            return View(dateres.ToList());
        }

        public async Task<IActionResult> Approuved()
        {
            
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Student"))
                {
                    await DataByUser();
                }
                else if (User.IsInRole("Admin"))
                {
                    Res_Approved();
                }
                else
                {
                    return NotFound();
                }
            }
            return View();

        }

        public async Task<IActionResult> Declined()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Student"))
                {
                    await DataByUser();
                }
                else if (User.IsInRole("Admin"))
                {
                    Res_Declined();
                }
                else
                {
                    return NotFound();
                }
            }
            return View();

        }

        public async Task<IActionResult> Pending()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Student"))
                {
                    await DataByUser();
                }
                else if (User.IsInRole("Admin"))
                {
                    Pend();
                }
                else
                {
                    return NotFound();
                }
            }
            return View();

        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Student"))
                {
                    await DataByUser();
                }
                else if (User.IsInRole("Admin"))
                {
                    Resby_res();
                }
                else
                {
                    return NotFound();
                }
            }
            return View();

        }

        

        public async Task<IActionResult> DataByUser()
        {
            var student = await _userManager.GetUserAsync(HttpContext.User);
            var list = _context.Reservations.Where(d=>d.Date>=DateTime.Today).OrderBy(r=>r.Date)
                .Include(s => s.Student)
                .Include(rt => rt.Reserv).Where(s => s.StudentId == student.Id).ToList();
            return View("Index",list);
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
            var u = _context.Students.FirstOrDefault(s => s.Id == usr.StudentId);
            var inc = usr.Student.ResCount;
            var st = usr.Status;
            if(st!= "Approved")
            {
                u.ResCount = inc + 1;
                _context.Update(usr);
                _context.Update(u);
                _context.SaveChanges();
            }
            
        }

        public void Decrement(int id)
        {
            var usr = _context.Reservations.Find(id);
            var u = _context.Students.FirstOrDefault(s => s.Id == usr.StudentId);
            var inc = usr.Student.ResCount;
            var st = usr.Status;
            if (st == "Approved")
            {
                u.ResCount = inc - 1;
                _context.Update(usr);
                _context.Update(u);
                _context.SaveChanges();
            }

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
                if(resr.Status == "Approved")
                {
                    Decrement(id);
                }
                
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
