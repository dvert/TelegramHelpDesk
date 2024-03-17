using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using TelegramHelpDesk.Models;
using EntityState = System.Data.Entity.EntityState;


namespace TelegramHelpDesk.Controllers
{

    public class UserController : Controller
    {
        private HelpdeskContext db = new HelpdeskContext();

        [HttpGet]
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Department).Include(u => u.Role).ToList();

            if (HttpContext.User.IsInRole("Администратор")) {

                List<Department> departments = db.Departments.ToList();
                departments.Insert(0, new Department { Name = "Все", Id = Guid.Empty });
                ViewBag.Departments = new SelectList(departments, "Id", "Name");

                List<Role> roles = db.Roles.ToList();
                roles.Insert(0, new Role { Name = "Все", Id = Guid.Empty });
                ViewBag.Roles = new SelectList(roles, "Id", "Name");

                }
            
           else {

                users = db.Users.Include(u => u.Department).Include(u => u.Role).ToList().FindAll(u => u.Name != "Администратор");

                List<Department> departments = db.Departments.ToList().FindAll(u => u.Name != "Администратор");
                departments.Insert(0, new Department { Name = "Все", Id = Guid.Empty });
                ViewBag.Departments = new SelectList(departments, "Id", "Name");

                List<Role> roles = db.Roles.ToList().FindAll(u => u.Name != "Администратор"); ;
                roles.Insert(0, new Role { Name = "Все", Id = Guid.Empty });
                ViewBag.Roles = new SelectList(roles, "Id", "Name");

                users.ForEach(x => { x.Login = null; x.Password = null; });

            }
            return View(users);
        }

        [HttpPost]
        public ActionResult Index(Guid department, Guid role)
        {
            IEnumerable<User> allUsers = null;
            if (role == Guid.Empty && department == Guid.Empty)
            {
                return RedirectToAction("Index");
            }
            if (role == Guid.Empty && department != Guid.Empty)
            {
                allUsers = from user in db.Users.Include(u => u.Department).Include(u => u.Role)
                           where user.DepartmentId == department
                           select user;
            }
            else if (role != Guid.Empty && department == Guid.Empty)
            {
                allUsers = from user in db.Users.Include(u => u.Department).Include(u => u.Role)
                           where user.RoleId == role
                           select user;
            }
            else
            {
                allUsers = from user in db.Users.Include(u => u.Department).Include(u => u.Role)
                           where user.DepartmentId == department && user.RoleId == role
                           select user;
            }

            List<Department> departments = db.Departments.ToList();
            departments.Insert(0, new Department { Name = "Все", Id = Guid.Empty });
            ViewBag.Departments = new SelectList(departments, "Id", "Name");

            List<Role> roles = db.Roles.ToList();
            roles.Insert(0, new Role { Name = "Все", Id = Guid.Empty });
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            return View(allUsers.ToList());
        }

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        public ActionResult Create()
        {
            SelectList departments = new SelectList(db.Departments, "Id", "Name");
            ViewBag.Departments = departments;
            SelectList roles = new SelectList(db.Roles, "Id", "Name");
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                user.Id = Guid.NewGuid();
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            SelectList departments = new SelectList(db.Departments, "Id", "Name");
            ViewBag.Departments = departments;
            SelectList roles = new SelectList(db.Roles, "Id", "Name");
            ViewBag.Roles = roles;

            return View(user);
        }

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        public ActionResult Edit(Guid id)
        {
            User user = db.Users.Find(id);
            SelectList departments = new SelectList(db.Departments, "Id", "Name", user.DepartmentId);
            ViewBag.Departments = departments;
            SelectList roles = new SelectList(db.Roles, "Id", "Name", user.RoleId);
            ViewBag.Roles = roles;

            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            SelectList departments = new SelectList(db.Departments, "Id", "Name");
            ViewBag.Departments = departments;
            SelectList roles = new SelectList(db.Roles, "Id", "Name");
            ViewBag.Roles = roles;

            return View(user);
        }

        [Authorize(Roles = "Администратор")]
        public ActionResult Delete(Guid id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}