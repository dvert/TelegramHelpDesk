using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TelegramHelpDesk.Models;
using System.Data.Entity;

namespace TelegramHelpDesk.Controllers
{
    public class ServiceController : Controller
    {
        HelpdeskContext db = new HelpdeskContext();

        //представление для просмотра отдела
        [HttpGet]
        public ActionResult Departments()
        {
            ViewBag.Departments = db.Departments;
            return View();
        }

        //добавление нового отдела
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

        //удаление отдела
        public ActionResult DeleteDepartment(Guid id)
        {
            Department depo = db.Departments.Find(id);
            db.Departments.Remove(depo);
            db.SaveChanges();
            return RedirectToAction("Departments");
        }

        //представление для просмотра категорий заявок
        [HttpGet]
        public ActionResult Categories()
        {
            SelectList dept = new SelectList(db.Departments, "Id", "Name");

            ViewBag.Departments = dept;
            ViewBag.Categories = db.Categorys;
            return View();
        }

        //добавление новых категорий
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

        //удаление категории
        public ActionResult DeleteCategory(Guid id)
        {
            Category cat = db.Categorys.Find(id);
            db.Categorys.Remove(cat);
            db.SaveChanges();
            return RedirectToAction("Categories");
        }

        // прредставление для просмотра районов и офисов
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

        //добавление районов и офисов
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

        // Удаление районов и офисов
        public ActionResult DeleteDistrict(Guid id)
        {
            District district = db.Districts.Find(id);
            db.Districts.Remove(district);
            db.SaveChanges();
            return RedirectToAction("Districts");
        }
    }
}