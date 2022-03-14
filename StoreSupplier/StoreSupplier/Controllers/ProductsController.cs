using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreSupplier.Infrastructure;
using StoreSupplier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSupplier.Controllers
{
    public class ProductsController : Controller
    {
        private readonly StoreSupplierContext context;

        public ProductsController(StoreSupplierContext context)
        {
            this.context = context;
        }


        //GET Request products
        public async Task<IActionResult> Index(int p = 1)
        {


            int pageSize = 6;
            var products = context.Products.OrderByDescending(x => x.Id) 
                                            .Skip((p - 1) * pageSize)
                                            .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Products.Count() / pageSize);

            return View(await products.ToListAsync());
        }

        //GET product/IndexSearch

        public async Task<IActionResult> IndexSearch(string searchString)
        {
            var products = from m in context.Products.Include(x => x.Category)
                           select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name!.Contains(searchString));
            }

            return View(await products.ToListAsync());
        }


        //GET Request products/category
        public async Task<IActionResult> ProductsByCategory(string categorySlug, int p = 1)
        {
            Category category = await context.Categories.Where(x => x.Slug == categorySlug).FirstOrDefaultAsync();

            if (category == null) return RedirectToAction("Index");

            int pageSize = 6;
            var products = context.Products.OrderByDescending(x => x.Id)
                                            .Where(x => x.CategoryId == category.Id)
                                            .Skip((p - 1) * pageSize)
                                            .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Products.Where(x => x.CategoryId == category.Id).Count() / pageSize);
            ViewBag.CategoryName = category.Name;
            ViewBag.CategorySlug = category.Slug;


            return View(await products.ToListAsync());
        }
    }
}
