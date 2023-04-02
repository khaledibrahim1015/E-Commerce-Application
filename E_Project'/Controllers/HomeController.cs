using E_Project_.Data;
using E_Project_.Models;
using E_Project_.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using Microsoft.AspNetCore.Http;
using E_Project_.Utility;
using static System.Collections.Specialized.BitVector32;

namespace E_Project_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = _db.Product.Include(c => c.Category).Include(p => p.ApplicationType),
                Categories = _db.Category
            };


            return View(homeVM);
        }


        // Get Prodcut 
        public IActionResult Details(int? id)
        {
            //if (id == null || id == 0) return NotFound();



            DetailsVM detailsVM = new DetailsVM()
            {
                Product = _db.Product.Include(c => c.Category).Include(ap => ap.ApplicationType).Where(p => p.Id == id).FirstOrDefault(),
                ExistsInCart = false
            };

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            // check if id exist in session or not 
            //  first retrieve all data from session 
            if (HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart) != null && HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart).Count() > 0)
            {
                //  if there are data in session => 
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart);
            }

            foreach (var item in shoppingCartList)
            {
                if (item.ProductId == id)
                {
                    detailsVM.ExistsInCart = true;
                }
            }

            return View(detailsVM);

        }
        // add to cart then we have product id 
        // add to session 

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            // list of shoppingcart => to retirve get data from session there are ids exist in session or not !

            List<ShoppingCart> ShoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart) != null
                && HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart).Count() > 0) // that is mean there are data in session  => that session Exist 
            {
                //  so we need to get or retrieve data from session  and add items to session
                // if session have id or ids  value or values
                // add all to list of shoppingcart 

                ShoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart);


            }
            // session empty we will add id (item ) in shopping cart and if have we will add more time  
            ShoppingCartList.Add(new ShoppingCart { ProductId = id });






            // then add it in session (set )

            HttpContext.Session.Set(WC.ShoppingCart, ShoppingCartList);

            return RedirectToAction(nameof(Index));




        }

        public IActionResult RemoveFromCart(int id)
        {

            List<ShoppingCart> shoppingCartList = new();

            if(HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart)!=null && HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart).Count()>0)
            {

                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart);
            }

            //  Item that want to remove from cart and remove it value from session 
            var itemToRemove = shoppingCartList.SingleOrDefault(s => s.ProductId == id);

            if(itemToRemove!=null)
            {
                // then we need to remove it from cart and remove it from session 
                shoppingCartList.Remove(itemToRemove);
            }

            // here we need to set session with new data 
            // need to set shopping cart again with the neew list 

            HttpContext.Session.Set(WC.ShoppingCart, shoppingCartList);

            return RedirectToAction(nameof(Index));



        }





    }
}
