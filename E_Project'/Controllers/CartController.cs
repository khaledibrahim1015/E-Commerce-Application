using E_Project_.Data;
using E_Project_.Models;
using E_Project_.Models.ViewModels;
using E_Project_.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Project_.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender )
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
        }


        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            //  Data in session 
            if(HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart)!=null && HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart).Count()>0)
            {
                //  session exist have data 
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart);

            }

            // create list of product to have ids exists in shoppingcartlist 
            List<int> ListOfProductIds = shoppingCartList.Select(s => s.ProductId).ToList();

            //List<Product> ProductList = (List<Product>)_db.Product.Where(p => ListOfProductIds.Contains(p.Id));
            IEnumerable<Product> ProductList =  _db.Product.Where(p => ListOfProductIds.Contains(p.Id));

        

            return View(ProductList);
        }


        [HttpPost]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {

            return RedirectToAction(nameof(Summary));
        }


        // Get
        public IActionResult Summary()
        {
            //  Get Id of user that logged in 
            //  by claims in cookies that user have 

            var claimsIdentity =(ClaimsIdentity) User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier); //  return user id 
            // oR

            //var userId = User.FindFirstValue(ClaimTypes.Name);



            List <ShoppingCart> shoppingCartList = new();

            if(HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart)!=null && HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart).Count()>0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart);

            }

            // List of ids that exist in shoppingCartList 
            List<int> ListOfProductIds = shoppingCartList.Select(p => p.ProductId).ToList();

            //  list of product 

            IEnumerable<Product> ProductList =_db.Product.Where(p => ListOfProductIds.Contains(p.Id));

            ProductUserVM productUserVM = new ProductUserVM()
            {
                //ApplicationUser = _db.ApplicationUser.FirstOrDefault(p => p.Id == userId)
                applicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value),
                ProductLst=ProductList.ToList()
            };

            return View(productUserVM);

        }

        // post Summary 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserVM productUserVM)
        {
            //  When submit button => send Email 
            // once Email send we will load confirmation page to show user that his inquiry has been submited !!
            //  using template 

            // Send Email Using INquiry Template 


            var pathToTemplate = _webHostEnvironment.WebRootPath + // represent wwwroot
                                Path.DirectorySeparatorChar.ToString() + // represent forward slash /
                                "templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";

            var Subject = "New Inquiry ";
            string HtmlBody = "";

            // Read Template and store it in HtmlBody

            using( StreamReader sr= System.IO.File.OpenText(pathToTemplate))
            {
                HtmlBody = sr.ReadToEnd(); //  here read file and store it in htmlbody 

            }


            /*
            
            Name : {0}
            Email  : {1}
            Phone : {2}
            Products Interested:
            {3}
          
             */

            // want to replace these numbers in htmlbody 


            StringBuilder productListSB = new StringBuilder();

            foreach (var product in productUserVM.ProductLst)
            {
                productListSB.Append($" Name : {product.Name} <span style='font-size:14px;'> (Id : {product.Id})</span><br/>");
            }

            string messageBody = string.Format(
                HtmlBody,
                productUserVM.applicationUser.FullName,
                productUserVM.applicationUser.Email,
                productUserVM.applicationUser.PhoneNumber,
                productListSB.ToString()
                );



            /// send email 

            await _emailSender.SendEmailAsync(WC.AdminEmail, Subject, messageBody);


            return RedirectToAction(nameof(InquiryConfirmation));



        }


        public IActionResult InquiryConfirmation()
        {
            // First clear Session that has data 
            HttpContext.Session.Clear();

            return View();
        }
















        public IActionResult Remove( int id)
        {
            List<ShoppingCart> shoppingCartList = new();

            if(HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart)!=null && HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart).Count()>0)
            {
                //  if exist data in session 
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.ShoppingCart);


            }
            // get product by id 
            var productInShoppingCartList = shoppingCartList.FirstOrDefault(p => p.ProductId == id);

            shoppingCartList.Remove(productInShoppingCartList);

            // Re Set session with new data 
            HttpContext.Session.Set(WC.ShoppingCart, shoppingCartList);

            return RedirectToAction(nameof(Index));





        }
    }
}
