using StoreSupplier.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StoreSupplier.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSupplier.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly StoreSupplierContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private IPasswordHasher<AppUser> passwordHasher;

        public AccountController
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



        [AllowAnonymous]
        //Get /account/register
        public IActionResult Register()
        {
            ViewBag.BranchId = new SelectList(context.Branches.OrderBy(x => x.Sorting), "Id", "Name");

            return View();
        }
        [AllowAnonymous]
        //POST /account/register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            ViewBag.BranchId = new SelectList(context.Branches.OrderBy(x => x.Sorting), "Id", "Name");


            //Check if Email exists in user db
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
                    return RedirectToAction("Login");
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


        // GET /account/login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            Login login = new Login
            {
                ReturnUrl = returnUrl
            };

            return View(login);
        }


        // Post /account/login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = await userManager.FindByEmailAsync(login.Email);
                if (appUser != null)
                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(appUser, login.Password, false, true);
                    if (result.Succeeded)
                        return Redirect(login.ReturnUrl ?? "/");

                    if (result.IsLockedOut)
                        ModelState.AddModelError("", "Your account is locked out try again later.");
                }
                ModelState.AddModelError("", "Login failed, wrong credentials.");
            }

            return View(login);
        }
        // GET /account/logout
        public async Task<IActionResult> Logout( )
        {
            await signInManager.SignOutAsync();

            return Redirect("/");
        }

        // GET /account/edit
        public async Task<IActionResult> Edit()
        {
            ViewBag.BranchId = new SelectList(context.Branches.OrderBy(x => x.Sorting), "Id", "Name");

            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);
            UserEdit user = new UserEdit(appUser);

            return View(user);
        }


        // Post /account/edit
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEdit user)
        {
            ViewBag.BranchId = new SelectList(context.Branches.OrderBy(x => x.Sorting), "Id", "Name");
            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);


            if (ModelState.IsValid)
            {
                appUser.Name = user.Name;
                appUser.LastName = user.LastName;
                appUser.Email = user.Email;
                appUser.BirthDate = user.BirthDate;


                if (user.Password != null)
                {
                    appUser.PasswordHash = passwordHasher.HashPassword(appUser, user.Password);
                }

                IdentityResult result = await userManager.UpdateAsync(appUser);
                if(result.Succeeded)
                {
                    TempData["Success"] = "Your account has been edited successfully!";
                }
            }

            return View();
        }

        //Get /account/AccessDenied
        public IActionResult AccessDenied() => View();

    }
}
