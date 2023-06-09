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
    
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ProductsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }


        // Se afiseaza lista tuturor articolelor impreuna cu categoria 
        // din care fac parte dar
        // Pentru fiecare articol se afiseaza si userul care a postat articolul respectiv
        // HttpGet implicit
        //[Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Index()
        {
            var products = db.products.Include("Category")
                                      .Include("User").OrderBy(a => a.Date);

            var search = "";

            // MOTOR DE CAUTARE

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim(); // eliminam spatiile libere 

                // Cautare in articol (Title si Content)

                List<int> ProductIDs = db.products.Where
                                        (
                                         at => at.Title.Contains(search)
                                         || at.Content.Contains(search)
                                        ).Select(a => a.Id).ToList();

                // Cautare in comentarii (Content)
                List<int> ProductIDsOfreviewsWithSearchString = db.reviews
                                        .Where
                                        (
                                         c => c.Content.Contains(search)
                                        ).Select(c => (int)c.ProductID).ToList();

                // Se formeaza o singura lista formata din toate id-urile selectate anterior
                List<int> mergedIds = ProductIDs.Union(ProductIDsOfreviewsWithSearchString).ToList();


                // Lista articolelor care contin cuvantul cautat
                // fie in articol -> Title si Content
                // fie in comentarii -> Content
                products = db.products.Where(product => mergedIds.Contains(product.Id))
                                      .Include("Category")
                                      .Include("User")
                                      .OrderBy(a => a.Date);

            }

            ViewBag.SearchString = search;

            // AFISARE PAGINATA

            // Alegem sa afisam 3 articole pe pagina
            int _perPage = 3;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }


            // Fiind un numar variabil de articole, verificam de fiecare data utilizand 
            // metoda Count()

            int totalItems = products.Count();


            // Se preia pagina curenta din View-ul asociat
            // Numarul paginii este valoarea parametrului page din ruta
            // /products/Index?page=valoare

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            // Pentru prima pagina offsetul o sa fie zero
            // Pentru pagina 2 o sa fie 3 
            // Asadar offsetul este egal cu numarul de articole care au fost deja afisate pe paginile anterioare
            var offset = 0;

            // Se calculeaza offsetul in functie de numarul paginii la care suntem
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            // Se preiau articolele corespunzatoare pentru fiecare pagina la care ne aflam 
            // in functie de offset
            var paginatedproducts = products.Where(a => a.Status == "Accepted").Skip(offset).Take(_perPage);


            // Preluam numarul ultimei pagini

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            // Trimitem articolele cu ajutorul unui ViewBag catre View-ul corespunzator
            ViewBag.products = paginatedproducts;

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/products/Index/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/products/Index/?page";
            }


            return View();
        }

        public IActionResult IndexPending()
        {
            var products = db.products.Include("Category")
                                      .Include("User").OrderBy(a => a.Date);

            var search = "";

            // MOTOR DE CAUTARE

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim(); // eliminam spatiile libere 

                // Cautare in articol (Title si Content)

                List<int> ProductIDs = db.products.Where
                                        (
                                         at => at.Title.Contains(search)
                                         || at.Content.Contains(search)
                                        ).Select(a => a.Id).ToList();

                // Cautare in comentarii (Content)
                List<int> ProductIDsOfreviewsWithSearchString = db.reviews
                                        .Where
                                        (
                                         c => c.Content.Contains(search)
                                        ).Select(c => (int)c.ProductID).ToList();

                // Se formeaza o singura lista formata din toate id-urile selectate anterior
                List<int> mergedIds = ProductIDs.Union(ProductIDsOfreviewsWithSearchString).ToList();


                // Lista articolelor care contin cuvantul cautat
                // fie in articol -> Title si Content
                // fie in comentarii -> Content
                products = db.products.Where(product => mergedIds.Contains(product.Id))
                                      .Include("Category")
                                      .Include("User")
                                      .OrderBy(a => a.Date);

            }

            ViewBag.SearchString = search;

            // AFISARE PAGINATA

            // Alegem sa afisam 3 articole pe pagina
            int _perPage = 3;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }


            // Fiind un numar variabil de articole, verificam de fiecare data utilizand 
            // metoda Count()

            int totalItems = products.Count();


            // Se preia pagina curenta din View-ul asociat
            // Numarul paginii este valoarea parametrului page din ruta
            // /products/Index?page=valoare

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            // Pentru prima pagina offsetul o sa fie zero
            // Pentru pagina 2 o sa fie 3 
            // Asadar offsetul este egal cu numarul de articole care au fost deja afisate pe paginile anterioare
            var offset = 0;

            // Se calculeaza offsetul in functie de numarul paginii la care suntem
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            // Se preiau articolele corespunzatoare pentru fiecare pagina la care ne aflam 
            // in functie de offset
            var paginatedproducts = products.Where(a => a.Status == "Pending").Skip(offset).Take(_perPage);


            // Preluam numarul ultimei pagini

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            // Trimitem articolele cu ajutorul unui ViewBag catre View-ul corespunzator
            ViewBag.products = paginatedproducts;

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/products/IndexPending/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/products/IndexPending/?page";
            }


            return View();
        }


        // Se afiseaza un singur articol in functie de id-ul sau 
        // impreuna cu categoria din care face parte
        // In plus sunt preluate si toate comentariile asociate unui articol
        // Se afiseaza si userul care a postat articolul respectiv
        // HttpGet implicit

        //[Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Show(int id)
        {
            product product = db.products.Include("Category")
                                         .Include("User")
                                         .Include("reviews")
                                         .Include("reviews.User")
                                         .Where(art => art.Id == id)
                                         .First();

            // Adaugam order-urile utilizatorului pentru dropdown
            ViewBag.Userorders = db.orders
                                      .Where(b => b.UserId == _userManager.GetUserId(User))
                                      .ToList();


            SetAccessRights();
            if (product.Status == "Accepted")
            { return View(product); }
            return View();
        }




        public IActionResult ShowPending(int id)
        {
            product product = db.products.Include("Category")
                                         .Include("User")
                                         .Include("reviews")
                                         .Include("reviews.User")
                                         .Where(art => art.Id == id)
                                         .First();

            // Adaugam order-urile utilizatorului pentru dropdown
            ViewBag.Userorders = db.orders
                                      .Where(b => b.UserId == _userManager.GetUserId(User))
                                      .ToList();


            SetAccessRights();
            if (product.Status == "Pending")
            { return View(product); }
            return View();
        }


        // Adaugarea unui comentariu asociat unui articol in baza de date
        [HttpPost]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Show([FromForm] review review)
        {
            review.Date = DateTime.Now;
            review.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.reviews.Add(review);
                db.SaveChanges();
                return Redirect("/products/Show/" + review.ProductID);
            }

            else
            {
                product art = db.products.Include("Category")
                                         .Include("User")
                                         .Include("reviews")
                                         .Include("reviews.User")
                                         .Where(art => art.Id == review.ProductID)
                                         .First();

                //return Redirect("/products/Show/" + comm.ProductID);

                // Adaugam order-urile utilizatorului pentru dropdown
                ViewBag.Userorders = db.orders
                                          .Where(b => b.UserId == _userManager.GetUserId(User))
                                          .ToList();

                SetAccessRights();

                return View(art);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Addorder([FromForm] productOrder productorder)
        {
            // Daca modelul este valid
            if(ModelState.IsValid)
            {
                // Verificam daca avem deja articolul in colectie
                if(db.productorders
                    .Where(ab => ab.ProductID == productorder.ProductID)
                    .Where(ab => ab.orderId == productorder.orderId)
                    .Count() > 0)
                {
                    TempData["message"] = "Acest articol este deja adaugat in colectie";
                    TempData["messageType"] = "alert-danger";
                }
                else
                {
                    // Adaugam asocierea intre articol si order 
                    db.productorders.Add(productorder);
                    // Salvam modificarile
                    db.SaveChanges();

                    // Adaugam un mesaj de success
                    TempData["message"] = "Articolul a fost adaugat in colectia selectata";
                    TempData["messageType"] = "alert-success";
                }
                
            } else
            {
                TempData["message"] = "Nu s-a putut adauga articolul in colectie";
                TempData["messageType"] = "alert-danger";
            }

            // Ne intoarcem la pagina articolului
            return Redirect("/products/Show/" + productorder.ProductID);
        }


        // Se afiseaza formularul in care se vor completa datele unui articol
        // impreuna cu selectarea categoriei din care face parte
        // Doar utilizatorii cu rolul de Colaborator sau Admin pot adauga articole in platforma
        // HttpGet implicit

        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            product product = new product();

            // Set the status based on the user's role
            string userRole = User.Identity.Name; // Get the user's role

            // Retrieve the list of categories using the GetAllCategories() method
            product.Status = "Accepted";

            product.Categ = GetAllCategories();

            return View(product);
        }
        [Authorize(Roles = "Colaborator,Admin")]
        public IActionResult NewPending()
        {
            product product = new product();

            // Set the status based on the user's role
            string userRole = User.Identity.Name; // Get the user's role
            product.Status = "Pending";

            // Retrieve the list of categories using the GetAllCategories() method

            product.Categ = GetAllCategories();

            return View(product);
        }

        // Se adauga articolul in baza de date
        // Doar utilizatorii cu rolul de Colaborator sau Admin pot adauga articole in platforma

        [Authorize(Roles = "User,Colaborator,Admin")]
        [HttpPost]
        
        public IActionResult New(product product)
        {
            var sanitizer = new HtmlSanitizer();

            product.Date = DateTime.Now;
            product.UserId = _userManager.GetUserId(User);


            if (ModelState.IsValid)
            {
                product.Content = sanitizer.Sanitize(product.Content);
                product.Poza = sanitizer.Sanitize(product.Poza);
                

                product.Status = sanitizer.Sanitize(product.Status);
                db.products.Add(product);
                db.SaveChanges();
                TempData["message"] = "Articolul a fost adaugat";
                return RedirectToAction("Index");
            }
            else
            {
                product.Categ = GetAllCategories();
                return View(product);
            }
        }

        // Se editeaza un articol existent in baza de date impreuna cu categoria
        // din care face parte
        // Categoria se selecteaza dintr-un dropdown
        // HttpGet implicit
        // Se afiseaza formularul impreuna cu datele aferente articolului
        // din baza de date
        [Authorize(Roles = "Colaborator,Admin")]
        public IActionResult Edit(int id)
        {

            product product = db.products.Include("Category")
                                        .Where(art => art.Id == id)
                                        .First();

            product.Categ = GetAllCategories();

            if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {


                return View(product);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                return RedirectToAction("Index");
            }

        }

        // Se adauga articolul modificat in baza de date
        [HttpPost]
        [Authorize(Roles = "Colaborator,Admin")]
        public IActionResult Edit(int id, product requestproduct)
        {
            var sanitizer = new HtmlSanitizer();

            product product = db.products.Find(id);
            

            if (ModelState.IsValid)
            {
                if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    product.Title = requestproduct.Title;

                    requestproduct.Content = sanitizer.Sanitize(requestproduct.Content);

                    product.Content = requestproduct.Content;

                    requestproduct.Poza = sanitizer.Sanitize(requestproduct.Poza);

                    product.Poza = requestproduct.Poza;

                    

                    product.CategoryId = requestproduct.CategoryId;
                    TempData["message"] = "Articolul a fost modificat";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                requestproduct.Categ = GetAllCategories();
                return View(requestproduct);
            }
        }


        // Se sterge un articol din baza de date 
        [HttpPost]
        [Authorize(Roles = "Colaborator,Admin")]

        public ActionResult Accept(int id)
        {
            product product = db.products.Include("reviews")
                                         .Where(art => art.Id == id)
                                         .First();

            
            
                product.Status = "Accepted";
                db.SaveChanges();
                TempData["message"] = "Articolul a fost acceptat";
                return RedirectToAction("IndexPending");
            


        }

        public ActionResult Delete(int id)
        {
            product product = db.products.Include("reviews")
                                         .Where(art => art.Id == id)
                                         .First();

            if (product.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.products.Remove(product);
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

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pentru dropdown
                // id-ul categoriei si denumirea acesteia
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
                /* Sau se poate implementa astfel: 
                 * 
                foreach (var category in categories)
                {
                    var listItem = new SelectListItem();
                    listItem.Value = category.Id.ToString();
                    listItem.Text = category.CategoryName.ToString();

                    selectList.Add(listItem);
                 }*/
            

            // returnam lista de categorii
            return selectList;
        }

        // Metoda utilizata pentru exemplificarea Layout-ului
        // Am adaugat un nou Layout in Views -> Shared -> numit _LayoutNou.cshtml
        // Aceasta metoda are un View asociat care utilizeaza noul layout creat
        // in locul celui default generat de framework numit _Layout.cshtml
        public IActionResult IndexNou()
        {
            return View();
        }

        // Conditiile de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Colaborator"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
    }
}

