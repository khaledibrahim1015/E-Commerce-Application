using E_Project_.Data;
using E_Project_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace E_Project_.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {
      private readonly   ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> ObjList = _db.Category;


            return View(ObjList);
        }

        // Get - Create 
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        // Post - Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if(ModelState.IsValid)
            {
                _db.Category.Add(obj);
                _db.SaveChanges();

                return RedirectToAction("Index");


            }
            else
            {
                return View(obj);
            }

           
        }

        // Get 
        [HttpGet]
        public IActionResult Edit(int? id )
        {
            // in case send id 

            if(id==null || id==0)
            {

                return NotFound();
            }
            Category selectedCategory = _db.Category.FirstOrDefault(c => c.Id == id);
            // if does not exist in db 
            if(selectedCategory==null)
            {
                return NotFound();
            }
            // else

            return View(selectedCategory);
        }

        // Post
        [HttpPost]
        public IActionResult Edit(int id,Category updatedCategory)
        {
            if(ModelState.IsValid)
            {
                //var oldObj = _db.Category.FirstOrDefault(c => c.Id == id);

                //oldObj.Name = updatedCategory.Name;
                //oldObj.DisplayOrder = updatedCategory.DisplayOrder;

                //_db.SaveChanges();

                // Or another way to update object using update method in entity framework 

                _db.Category.Update(updatedCategory);
                _db.SaveChanges();


                //// Or 
                //_db.Entry(updatedCategory).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                //_db.SaveChanges();



                return RedirectToAction("Index");
               
            }
            else
            {
                return View(updatedCategory);

            }


        }








        // Get - DELETE 
        [HttpGet]
        public IActionResult Delete(int? id )
        {
            if(id == null || id == 0)
            {

                return NotFound();
            }
            Category SelectedCategory = _db.Category.SingleOrDefault(c => c.Id == id);
            if(SelectedCategory==null)
            {
                return NotFound();
            }


            return View(SelectedCategory);

        }


        // Post
        [HttpPost]
        public IActionResult DeletePost(int? id)
        {

            var obj = _db.Category.SingleOrDefault(c => c.Id == id);
            if (obj == null)
            {
                return NotFound();
            }


            //_db.Category.Remove(obj);
            //_db.SaveChanges();


            // Or
            _db.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            _db.SaveChanges();

            return RedirectToAction("Index");


        }










    }
}
