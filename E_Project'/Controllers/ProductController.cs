using E_Project_.Data;
using E_Project_.Models;
using E_Project_.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace E_Project_.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
       private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment = null)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }




        public IActionResult Index()
        {

            //// first way to get related data 

            //var objLst = _db.Product;
            //foreach (var item in objLst)
            //{
            //    // object from category 
            //    item.Category = _db.Category.FirstOrDefault(u => u.Id == item.CategoryId);
            //}


            IEnumerable<Product> objLst = _db.Product.Include(p => p.Category).Include(ap=>ap.ApplicationType).ToList();

            return View(objLst);


        }


        ////1- this using losely typed View 

        //// UpSert => for Edit Update and create insert in the same action 
        ////Get
        //public IActionResult UpSert(int? id)
        //{
        //    //// 1- For DropDownList
        //    //List<Category> CategoryDropDownList = _db.Category.ToList(); // datasource 


        //    //ViewBag.CategoryDropDownList = CategoryDropDownList;

        //    //2- or another Way to DropDownList

        //    IEnumerable<SelectListItem> CategoryDropDownList = _db.Category.Select(c => new SelectListItem
        //    {
        //        Text = c.Name, // Display item 
        //        Value = c.Id.ToString()
        //    }
        //    );
        //    ViewBag.CategoryDropDownList = CategoryDropDownList;


        //    // in case  null that is mean for create new product 

        //    Product product = new Product();

        //    if(id==null )
        //    {
        //        //  Create 
        //        return View(product);


        //    }
        //    else
        //    {
        //        // id !=null that mean for update 
        //        product = _db.Product.Find(id);
        //        if(product==null)
        //        {
        //            return NotFound();

        //        }
        //        return View(product);


        //    }
      


        //}





        // 2- this using Strong Types View 
        // Using ViewModel
        public IActionResult UpSert(int? id)
        {

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectListItems = _db.Category.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }
                ),
                ApplicationSelectListItems=_db.ApplicationType.Select(ap=>new SelectListItem 
                { 
                    Text=ap.Name,
                    Value=ap.Id.ToString()
                })
            };





            if (id == null)
            {
                //  Create 
                return View(productVM);


            }
            else
            {
                // id !=null that mean for update 
                productVM.Product = _db.Product.Find(id);
                if (productVM.Product == null)
                {
                    return NotFound();

                }
                return View(productVM);


            }



        }

        // Post
        [HttpPost]
        public IActionResult UpSert(ProductVM productVM)
        {

            if(ModelState.IsValid)
            {
                // if a new image uploaded we retrieve that and save it in files 
                var files = HttpContext.Request.Form.Files;

                //  to access wwwroot using inject iwebhostEnvironment 

                string WebRootPath = _webHostEnvironment.WebRootPath;

                // we have same action for update and create 
                //  if id =0 => create else have id then for=> update 
                if(productVM.Product.Id==0)
                {
                    // create new Product 

                    // full path for folder to save image in a server 
                    string Upload = WebRootPath + WC.ImagePath;

                    // fileName want to give => cuz file uploaded with other name 
                    // new file name to save in server 
                    // use globaluniversalid = > to give it a unique name file on server 
                    string fileName = Guid.NewGuid().ToString();
                    // to get extension for file that upload 
                    string Extension = Path.GetExtension(files[0].FileName);

                    string FakeFileName = fileName + Extension;


                    using (var fileStream = new FileStream(Path.Combine(Upload, FakeFileName), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    // update image prop in product model 
                    //that meaan update productVm before insert obj product in db 
                    // update it with newfilename => fakeFileName that create using guid

                    productVM.Product.Image = FakeFileName;



                    // insert product in database 
                    _db.Product.Add(productVM.Product);
                    



                }
                else
                {
                    //  Update an existing product 
                    // first make sure that  if image is updated and we need to remove the old image and replace it with new image 
                    // and retrieve entity from db and update its property 


                    // first selected obj from db 

                    var objFromDb = _db.Product.AsNoTracking().SingleOrDefault(p => p.Id == productVM.Product.Id);


                    if(files.Count>0) // this means that a new file has been updated for an existing product obj 
                    {
                        // so we need to craete a new file 
                         WebRootPath = _webHostEnvironment.WebRootPath; // wwwroot webrootpath
                        string upload = WebRootPath+ WC.ImagePath;    // images/ product inside wwwroot => path 
                        string fileName = Guid.NewGuid().ToString();   // new filename
                        string Extension = Path.GetExtension(files[0].FileName); //extension
                        string FakeFileName = fileName + Extension; // newfilename+extension that save in server 

                        // Remove old file from server 
                        // remember that we store in db just newfilename+ Extension => fakeFileName 

                        //here we get full path for old image that stored in db and server 
                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        //check if file exist on server 
                        if(System.IO.File.Exists(oldFile))
                        {
                            // if file exist delete it 
                            System.IO.File.Delete(oldFile);
                        }



                        // create a new file 
                        using (var filestream = new FileStream(Path.Combine(upload, fileName+ Extension), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }

                        // update image prop in product model 
                        // update image prop in product model 
                        //that meaan update productVm before insert obj product in db 
                        // update it with newfilename => fakeFileName that create using guid

                        //productVM.Product.Image = FakeFileName;
                        productVM.Product.Image = fileName + Extension;




                    }
                    else
                    {
                        // if count>0 that mean the product updated  that mean image updated 
                        // else  that mean that is not updated  image still not updated
                        //بمعني اصح هسيبه زي ما هو 

                        productVM.Product.Image = objFromDb.Image;




                    }
                    // at the end  update model in db 
                    _db.Product.Update(productVM.Product);

                    // note entity frameWork Keep tracking entity as long as exist 
                    // that mean there is an error will happen when run 
                    // because EF can not tracking the same entity 
                    // in line 214  var objFromDb = _db.Product.SingleOrDefault(p => p.Id == productVM.Product.Id); and line 271 _db.Product.Update(productVM.Product);
                    // WHwn update shouid entity will update two entity has same id !!!!
                    //  so to solve we need ef keep tracking in update not in retrive data 
                    // var objFromDb = _db.Product.AsNoTracking().SingleOrDefault(p => p.Id == productVM.Product.Id);



                }
                _db.SaveChanges();
                return RedirectToAction("Index");


            }




            // Fill DropDownList
           productVM.CategorySelectListItems = _db.Category.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }
            );

            // Fill DropDownList
            productVM.ApplicationSelectListItems = _db.Category.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }
             );
            return View(productVM);

        }




        public IActionResult Delete (int? id )
        {
            if(id==null || id==0)
            {
                return NotFound();
            }

            // then we pass it two view with dorpdown list => using ViewModel 

            ProductVM productVM = new ProductVM()
            {
                Product = _db.Product.Find(id),
                CategorySelectListItems = _db.Category.Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                ApplicationSelectListItems = _db.ApplicationType.Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),

            };
            if(productVM.Product==null )
            {
                return NotFound();
            }
            return View(productVM);

        }

        [HttpPost]
        public IActionResult Delete(ProductVM productVM)
        {
            
         
                // Retrive obj from db 

             var objFromDb = _db.Product.AsNoTracking().FirstOrDefault(p => p.Id == productVM.Product.Id);
            if(objFromDb!=null)
            { 
                // First Remove Image From server 
                // bUt image stored  in db just fakeFileName = fileGuid+Extension 
                //  Need all Full Path => wwwroot+images+product =wc
                string WebRootPath = _webHostEnvironment.WebRootPath;
                string Upload = WebRootPath +WC.ImagePath;

                string fullpath = Path.Combine(Upload, objFromDb.Image);

                if(System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }

                // Delete from db 
                _db.Product.Remove(productVM.Product);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));

            }
            return NotFound();



        }

    }
}
