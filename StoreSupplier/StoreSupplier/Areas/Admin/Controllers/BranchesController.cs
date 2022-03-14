using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreSupplier.Infrastructure;
using StoreSupplier.Models;

namespace StoreSupplier
{
    [Authorize(Roles = "Admin,Employee")]
    [Area("Admin")]
    public class BranchesController : Controller
    {
        private readonly StoreSupplierContext context;

        public BranchesController(StoreSupplierContext context)
        {
            this.context = context;
        }

        //GET Request /admin/branches
        public async Task<IActionResult> Index(int p = 1)
        {
            int pageSize = 6;
            var branches = context.Branches.Where(x => x.Id != 9).OrderBy(x => x.Id)
                                            .Skip((p - 1) * pageSize)
                                            .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Branches.Count() / pageSize);


            return View(await branches.ToListAsync());
        }

        // GET: Branches/Details/5
        public async Task<IActionResult> Details(int id)
        {
            Branch branch = await context.Branches.FirstOrDefaultAsync(x => x.Id == id);
            if (branch == null)
            {
                return NotFound();
            }
            return View(branch);
        }

        // GET: Branches/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Branches/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Branch branch)
        {
            if (ModelState.IsValid)
            {
                branch.Slug = branch.Name.ToLower().Replace(" ", "-");
                branch.Sorting = 100;

                var slug = await context.Pages.FirstOrDefaultAsync(x => x.Slug == branch.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The page already exist");
                    return View(branch);
                }

                context.Add(branch);
                await context.SaveChangesAsync();

                TempData["Success"] = "The page has been added!";

                return RedirectToAction("Index");

            }
            return View(branch);
        }

        // GET: Branches/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            Branch branch = await context.Branches.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            return View(branch);
        }

        // POST: Branches/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Branch branch)
        {
            if (ModelState.IsValid)
            {
                branch.Slug = branch.Id == 1 ? "home" : branch.Name.ToLower().Replace(" ", "-");

                var slug = await context.Pages.Where(x => x.Id != branch.Id).FirstOrDefaultAsync(x => x.Slug == branch.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The branch already exist");
                    return View(branch);
                }

                context.Update(branch);
                await context.SaveChangesAsync();

                TempData["Success"] = "The branch has been edited!";

                return RedirectToAction("Edit", new { id = branch.Id });

            }

            return View(branch);

        }

        // GET: Branches/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            Branch branch = await context.Branches.FindAsync(id);

            if (branch == null)
            {
                TempData["Error"] = "This branch does not exist!";
            }
            else if(branch.Id == 9)
            {
                TempData["Error"] = "You can't delete this branch!!";
            }
            else
            {
                context.Branches.Remove(branch);
                await context.SaveChangesAsync();

                TempData["Success"] = "This branch has been deleted!";
            }
            return RedirectToAction("Index");
        }

        //Post Request /admin/pages/reorder
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1;

            foreach (var branchId in id)
            {
                Branch branch = await context.Branches.FindAsync(branchId);
                branch.Sorting = count;
                context.Update(branch);

                await context.SaveChangesAsync();
                count++;
            }
            return Ok();
        }
    }
}
