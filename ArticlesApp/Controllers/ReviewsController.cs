using productsApp.Data;
using productsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace productsApp.Controllers
{
    public class reviewsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public reviewsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        /*
        
        // Adaugarea unui comentariu asociat unui articol in baza de date
        [HttpPost]
        public IActionResult New(review comm)
        {
            comm.Date = DateTime.Now;

            if(ModelState.IsValid)
            {
                db.reviews.Add(comm);
                db.SaveChanges();
                return Redirect("/products/Show/" + comm.ProductID);
            }

            else
            {
                return Redirect("/products/Show/" + comm.ProductID);
            }

        }

        
        */


        // Stergerea unui comentariu asociat unui articol din baza de date
        [HttpPost]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Delete(int id)
        {
            review comm = db.reviews.Find(id);

            if(comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.reviews.Remove(comm);
                db.SaveChanges();
                return Redirect("/products/Show/" + comm.ProductID);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti comentariul";
                return RedirectToAction("Index", "products");
            }         
        }

        // In acest moment vom implementa editarea intr-o pagina View separata
        // Se editeaza un comentariu existent
        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Edit(int id)
        {
            review comm = db.reviews.Find(id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(comm);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati comentariul";
                return RedirectToAction("Index", "products");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Edit(int id, review requestreview)
        {
            review comm = db.reviews.Find(id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    comm.Content = requestreview.Content;

                    db.SaveChanges();

                    return Redirect("/products/Show/" + comm.ProductID);
                }
                else
                {
                    return View(requestreview);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "products");
            }
        }
    }
}