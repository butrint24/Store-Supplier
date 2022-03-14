using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreSupplier.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreSupplier.Models;
using Microsoft.AspNetCore.Authorization;

namespace StoreSupplier.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    [Area("Admin")]
    public class PagesController : Controller
    {
        private readonly StoreSupplierContext context;

        public PagesController(StoreSupplierContext context)
        {
            this.context = context;
        }
        //GET /admin/pages
        public async Task<IActionResult> Index(int p = 1)
        {
            int pageSize = 6;
            var pages = context.Pages.OrderBy(x => x.Sorting)
                                            .Skip((p - 1) * pageSize)
                                            .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Pages.Count() / pageSize);

            return View(await pages.ToListAsync());
        }

        //GET /admin/pages/details/5
        public async Task<IActionResult> Details(int id)
        {
            Page page = await context.Pages.FirstOrDefaultAsync(x => x.Id == id);
            if (page == null)
            {
                return NotFound();
            }
            return View(page);
        }

        //GET  /admin/pages/create
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        //Post  /admin/pages/create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Create(Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Title.ToLower().Replace(" ", "-");
                page.Sorting = 100;

                var slug = await context.Pages.FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The page already exist");
                    return View(page);
                }

                context.Add(page);
                await context.SaveChangesAsync();

                TempData["Success"] = "The page has been added!";

                return RedirectToAction("Index");

            }
            return View(page);
        }

        //Get Request /admin/product/edit/id
        public async Task<IActionResult> Edit(int id)
        {
            Page page = await context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        //Post Request /admin/pages/edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Id == 1 ? "home" : page.Title.ToLower().Replace(" ", "-");

                var slug = await context.Pages.Where(x => x.Id != page.Id).FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The page already exist");
                    return View(page);
                }

                context.Update(page);
                await context.SaveChangesAsync();

                TempData["Success"] = "The page has been edited!";

                return RedirectToAction("Edit", new { id = page.Id });

            }

            return View(page);

        }

        //Get Request /admin/pages/delete/id
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            Page page = await context.Pages.FindAsync(id);

            if (page == null)
            {
                TempData["Error"] = "The page does not exist!";
            }
            else
            {
                context.Pages.Remove(page);
                await context.SaveChangesAsync();

                TempData["Success"] = "The page has been deleted!";
            }
            return RedirectToAction("Index");
        }

        //Post Request /admin/pages/reorder
        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1;

            foreach (var pageId in id)
            {
                Page page = await context.Pages.FindAsync(pageId);
                page.Sorting = count;
                context.Update(page);

                await context.SaveChangesAsync();
                count++;
            }
            return Ok();
        }
    }
} 
