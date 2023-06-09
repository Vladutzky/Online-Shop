using productsApp.Controllers;
using productsApp.Data;
using productsApp.Models;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq;
namespace productsApp.Controllers
{
    public class PendingController : Controller
    {

        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public PendingController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }


        public IActionResult Index()
        {
            return View();
        }


        [Authorize(Roles = "Colaborator,Admin")]
        public IActionResult Accept(int id)
        {

            Pending pending = db.Pending.Include("Category")
                                        .Where(art => art.Id == id)
                                        .First();

            

            if (pending.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                product product = new product();
                product.UserId = pending.UserId;
                product.Title = pending.Title;
                product.Content = pending.Content;
                product.Poza = pending.Poza;
                product.Stele = pending.Stele;
                product.Date = pending.Date;
                product.Category = pending.Category;
                product.CategoryId = pending.CategoryId;
                product.User = pending.User;
                db.products.Add(product);
                TempData["message"] = "Articolul a fost mutat";
                db.Pending.Remove(pending);
                db.SaveChanges();


                return View(product);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                return RedirectToAction("Index");
            }

        }




        [HttpPost]
        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult Delete(int id)
        {
            Pending pending = db.Pending.Include("reviews")
            .Where(art => art.Id == id)
                                         .First();

            if (pending.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Pending.Remove(pending);
                db.SaveChanges();
                TempData["message"] = "Articolul a fost sters";
                return RedirectToAction("Index");
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un articol care nu va apartine";
                return RedirectToAction("Index");
            }
        }



    }
}
