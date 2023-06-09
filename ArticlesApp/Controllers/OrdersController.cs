using productsApp.Data;
using productsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace productsApp.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public OrdersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        // toti utilizatorii pot vedea order-urile existente in platforma
        // fiecare utilizator vede order-urile pe care le-a creat
        // HttpGet - implicit
        public IActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            SetAccessRights();

            if (User.IsInRole("User") || User.IsInRole("Colaborator"))
            {
                var orders = from order in db.orders.Include("User")
                               .Where(b => b.UserId == _userManager.GetUserId(User))
                                select order;

                ViewBag.orders = orders;

                return View();
            }
            else 
            if(User.IsInRole("Admin"))
            {
                var orders = from order in db.orders.Include("User")
                                select order;

                ViewBag.orders = orders;

                return View();
            }

            else
            {
                TempData["message"] = "Nu aveti drepturi";
                return RedirectToAction("Index", "products");
            }
            
        }

        // Afisarea tuturor articolelor pe care utilizatorul le-a salvat in 
        // order-ul sau 

        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Show(int id)
        {
            SetAccessRights();

            if (User.IsInRole("User") || User.IsInRole("Colaborator"))
            {
                var orders = db.orders
                                  .Include("productorders.product.Category")
                                  .Include("productorders.product.User")
                                  .Include("User")
                                  .Where(b => b.Id == id)
                                  .Where(b => b.UserId == _userManager.GetUserId(User))
                                  .FirstOrDefault();
                
                if(orders == null)
                {
                    TempData["message"] = "order-ul nu exista sau nu aveti drepturi";
                    return RedirectToAction("Index", "products");
                }

                return View(orders);
            }

            else 
            if (User.IsInRole("Admin"))
            {
                var orders = db.orders
                                  .Include("productorders.product.Category")
                                  .Include("productorders.product.User")
                                  .Include("User")
                                  .Where(b => b.Id == id)
                                  .FirstOrDefault();


                if (orders == null)
                {
                    TempData["message"] = "Resursa cautata nu poate fi gasita";
                    return RedirectToAction("Index", "products");
                }


                return View(orders);
            }

            else
            {
                TempData["message"] = "Nu aveti drepturi";
                return RedirectToAction("Index", "products");
            }  
        }


        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public ActionResult New(Order bm)
        {
            bm.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.orders.Add(bm);
                db.SaveChanges();
                TempData["message"] = "Colectia a fost adaugata";
                return RedirectToAction("Index");
            }

            else
            {
                return View(bm);
            }
        }


        // Conditiile de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Colaborator") || User.IsInRole("User"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
    }
}
