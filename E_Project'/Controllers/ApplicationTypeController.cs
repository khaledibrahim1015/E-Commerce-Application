using E_Project_.Data;
using E_Project_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace E_Project_.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ApplicationTypeController : Controller
    {


        private readonly ApplicationDbContext _db;

        public ApplicationTypeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objLst = _db.ApplicationType;

            return View(objLst);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ApplicationType applicationType = new();
            return View(applicationType);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType applicationType)
        {
            if(ModelState.IsValid)
            {
                _db.ApplicationType.Add(applicationType);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));


            }
            return View(applicationType);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(id==null||id==0)
            {
                return NotFound();
            }
            var selectedObj = _db.ApplicationType.Find(id);
            if(selectedObj==null)
            {
                return NotFound();

            }
            return View(selectedObj);



        }


        [HttpPost]
        public IActionResult Edit(ApplicationType obj)
        {
            if(ModelState.IsValid)
            {
                _db.ApplicationType.Update(obj);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(obj);


        }


        [HttpGet]
        public IActionResult Delete (int? id )
        {
            if(id==null|| id==0)
            {
                return NotFound();

            }
            var obj = _db.ApplicationType.SingleOrDefault(a => a.Id == id);
            if (obj != null)
                return View(obj);
            else return NotFound();
        }
        [HttpPost]
        public IActionResult Delete(ApplicationType obj)
        {
            var selectobj = _db.ApplicationType.SingleOrDefault(a => a.Id == obj.Id);
            if(selectobj!=null)
            {
                _db.Entry(selectobj).State = EntityState.Deleted;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();




        }

    }
}
