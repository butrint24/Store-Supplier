using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreSupplier.Infrastructure;
using StoreSupplier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace StoreSupplier.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly StoreSupplierContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private IPasswordHasher<AppUser> passwordHasher;

        public UsersController
            (
            StoreSupplierContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IPasswordHasher<AppUser> passwordHasher
            )
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.passwordHasher = passwordHasher;
        }


        //Get admin/users
        public IActionResult Index()
        {
            return View(userManager.Users.Where(x => x.BranchId != 9).OrderBy(x => x.Name));
        }

        //Get admin/users/clients
        public IActionResult Clients()
        {
            return View(userManager.Users.Where(x => x.BranchId == 9).OrderBy(x => x.Name));
        }


        //Get admin/users/RegisterStaff
        public IActionResult RegisterStaff()
        {
            ViewBag.BranchId = new SelectList(context.Branches.Where(x => x.Id != 9).OrderBy(x => x.Sorting), "Id", "Name");

            return View();
        }

        //POST admin/users/RegisterStaff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStaff(User user)
        {
            ViewBag.BranchId = new SelectList(context.Branches.Where(x => x.Id != 9).OrderBy(x => x.Sorting), "Id", "Name");

            var email = await userManager.FindByEmailAsync(user.Email);
            if (email != null)
            {
                ModelState.AddModelError("", "The email already exist");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    Name = user.Name,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    BirthDate = user.BirthDate,
                    PersonalNumber = user.PersonalNumber,
                    BranchId = user.BranchId
                };

                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(user);

        }

        //Get Request /admin/users/edit/id
        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.BranchId = new SelectList(context.Branches.Where(x => x.Id != 9).OrderBy(x => x.Sorting), "Id", "Name");

            AppUser appUser = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            StaffEdit user = new StaffEdit(appUser);

            return View(user);
        }

        // Post /admin/users/edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StaffEdit user, string id)
        {
            ViewBag.BranchId = new SelectList(context.Branches.Where(x => x.Id != 9).OrderBy(x => x.Sorting), "Id", "Name");
            AppUser appUser = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            

            if (ModelState.IsValid)
            {
                appUser.Name = user.Name;
                appUser.LastName = user.LastName;
                appUser.UserName = user.UserName;
                appUser.Email = user.Email;
                appUser.BranchId = user.BranchId;
                appUser.BirthDate = user.BirthDate;
                appUser.PersonalNumber = user.PersonalNumber;
                appUser.BranchId = user.BranchId;



                if (user.Password != null)
                {
                    appUser.PasswordHash = passwordHasher.HashPassword(appUser, user.Password);
                }

                IdentityResult result = await userManager.UpdateAsync(appUser);
                if (result.Succeeded)
                {
                    TempData["Success"] = "User has been edited successfully!";
                }
            }

            return View();
        }


        //GET /admin/users/details/5
        public async Task<IActionResult> Details(StaffEdit user, string id)
        {
            ViewBag.BranchId = new SelectList(context.Branches.OrderBy(x => x.Sorting), "Id", "Name");


            AppUser appUser = await userManager.Users.Include(x => x.Branch).FirstOrDefaultAsync(x => x.Id == id);


            StaffDetails userDetails = new StaffDetails(appUser);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(userDetails);
        }


        //Get Request /admin/users/delete/id
        public async Task<IActionResult> Delete(StaffEdit user, string id)
        {
            AppUser appUser = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            StaffEdit userDelete = new StaffEdit(appUser);

            if (userDelete == null)
            {
                TempData["Error"] = "The user does not exist!";
            }
            else
            {

                await userManager.DeleteAsync(appUser);

                TempData["Success"] = "The user has been deleted!";
            }
            return RedirectToAction("Index");
        }

        //Get Request /admin/users/clients/delete/id
        public async Task<IActionResult> DeleteClients(UserEdit user, string id)
        {
            AppUser appUser = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            StaffEdit userDelete = new StaffEdit(appUser);

            if (userDelete == null)
            {
                TempData["Error"] = "The user does not exist!";
            }
            else
            {

                await userManager.DeleteAsync(appUser);

                TempData["Success"] = "The user has been deleted!";
            }
            return RedirectToAction("Clients");
        }
    }
}
