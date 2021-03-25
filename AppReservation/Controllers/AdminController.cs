using AppReservation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppReservation.Controllers
{
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index() 
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(Role role)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = role.RoleName
                };
                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "admin");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(role);
        }


        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"the Role with id = {id} not found";
                return NotFound();
            }

            var identity = new Role
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach (var user in userManager.Users.ToList())
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    identity.Users.Add(user.UserName);
                }
            }
            return View(identity);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(Role rl)
        {
            var role = await roleManager.FindByIdAsync(rl.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"the Role with id = {rl.Id} not found";
                return NotFound();
            }
            else
            {
                role.Name = rl.RoleName;
                var update = await roleManager.UpdateAsync(role);
                if (update.Succeeded)
                {
                    return RedirectToAction("index");
                }

                foreach (var error in update.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(rl);
            }


        }


        public async Task<IActionResult> EditUserInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} not found";
                return NotFound();
            }

            var userModel = new List<UserRole>();

            foreach (var user in userManager.Users.ToList())
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRole.IsChecked = true;
                }
                else
                {
                    userRole.IsChecked = false;
                }
                userModel.Add(userRole);
            }
            return View(userModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UserRole> userRoles, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} not found";
                return NotFound();
            }

            for (int i = 0; i < userRoles.Count; i++)
            {
                var user = await userManager.FindByIdAsync(userRoles[i].UserId);
                IdentityResult result = null;
                if (userRoles[i].IsChecked && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!userRoles[i].IsChecked && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < userRoles.Count - 1)
                        continue;
                    else
                        return RedirectToAction("EditRole", new { id = roleId });
                }
            }
            return RedirectToAction("EditRole", new { id = roleId });
        }
    }
}
