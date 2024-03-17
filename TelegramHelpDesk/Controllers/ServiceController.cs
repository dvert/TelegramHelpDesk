using System;
using System.Linq;
using System.Web.Mvc;
using TelegramHelpDesk.Models;

namespace TelegramHelpDesk.Controllers
{
    public class ServiceController : Controller
    {
        HelpdeskContext db = new HelpdeskContext();

        [HttpGet]
        public ActionResult Departments()
        {
            ViewBag.Departments = db.Departments;
            return View();
        }

        [HttpPost]
        public ActionResult Departments(Department depo)
        {
            if (ModelState.IsValid) {
                depo.Id = Guid.NewGuid();
                db.Departments.Add(depo);
                db.SaveChanges();
            }

            ViewBag.Departments = db.Departments;
            return View(depo);
        }

        public ActionResult DeleteDepartment(Guid id)
        {
            Department depo = db.Departments.Find(id);
            db.Departments.Remove(depo);
            db.SaveChanges();
            return RedirectToAction("Departments");
        }

        [HttpGet]
        public ActionResult Categories()
        {
            SelectList dept = new SelectList(db.Departments, "Id", "Name");

            ViewBag.Departments = dept;
            ViewBag.Categories = db.Categorys;
            return View();
        }

        [HttpPost]
        public ActionResult Categories(Category cat)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();

            if (ModelState.IsValid)
            {
                cat.Id = Guid.NewGuid();
                db.Categorys.Add(cat);
                db.SaveChanges();
            }

            SelectList dept = new SelectList(db.Departments, "Id", "Name");
            ViewBag.Departments = dept;
            ViewBag.Categories = db.Categorys;
            return View();
        }

        public ActionResult DeleteCategory(Guid id)
        {
            Category cat = db.Categorys.Find(id);
            db.Categorys.Remove(cat);
            db.SaveChanges();
            return RedirectToAction("Categories");
        }

        [HttpGet]
        public ActionResult Districts()
        {
            ViewBag.Districts1 = db.Districts;
           
            SelectList districts = new SelectList(db.Districts.GroupBy(d => d.DistrictName)
                .Select(di => di.FirstOrDefault())
                .ToList(), "Id", "DistrictName");
            
            ViewBag.Districts = districts;
            return View();
        }

        [HttpPost]
        public ActionResult Districts(District district)
        {
            if (ModelState.IsValid)
            {
                District district1 = db.Districts.Find(Guid.Parse(district.DistrictName));
                district.Id = Guid.NewGuid();
                district.DistrictName = district1.DistrictName;
                db.Districts.Add(district);
                db.SaveChanges();
            }

            ViewBag.Districts1 = db.Districts;
            SelectList districts = new SelectList(db.Districts, "Id", "DistrictName");
            ViewBag.Districts = districts;
            return View(district);
        }

        public ActionResult DeleteDistrict(Guid id)
        {
            District district = db.Districts.Find(id);
            db.Districts.Remove(district);
            db.SaveChanges();
            return RedirectToAction("Districts");
        }
    }
}