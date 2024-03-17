using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TelegramHelpDesk.Hubs;
using TelegramHelpDesk.Models;
using EntityState = System.Data.Entity.EntityState;


namespace TelegramHelpDesk.Controllers
{
    [Authorize]
    public class AppTaskController : Controller
    {
        HelpdeskContext db = new HelpdeskContext();

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class MultiButtonAttribute : ActionNameSelectorAttribute
        {
            public string MatchFormKey { get; set; }
            public string MatchFormValue { get; set; }
            public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
            {
                return controllerContext.HttpContext.Request[MatchFormKey] != null && controllerContext.HttpContext.Request[MatchFormKey] == MatchFormValue;
            }
        }

        public ActionResult GetOfficeItems(Guid id) {

            District district = db.Districts.Find(id);
            return PartialView(db.Districts.Where(c => c.DistrictName == district.DistrictName).ToList());
        }

        public ActionResult GetCategoryItems(Guid id)
        {
            return PartialView(db.Categorys.Where(c => c.DepartmentId == id).ToList());
        }


        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "enter_departmentTask")]
        public ActionResult Get(AppTask request, HttpPostedFileBase error) {

            var tasks = 0;

            if (!db.AppTasks.Select(r => r.Number).Any())
            {
                tasks = 1;
            }
            else
            {
                tasks = db.AppTasks.Select(r => r.Number).Max() + 1;
            }

            User user = db.Users.FirstOrDefault(m => m.Login == HttpContext.User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("LogOff", "Account");
            }

            request.Id = Guid.NewGuid();
            request.Number = tasks;
            request.District = "нет";
            request.Office = "нет";
            var requestId = request.Id;
            request.CreateUserId = user.Id;
            var executorId = user.Id;
            request.Priority = Convert.ToInt32(Request.Form.GetValues("prio").FirstOrDefault());
            request.DepartmentId = db.Users.Find(request.ExecutorId)?.DepartmentId;

            var subject = db.Categorys.Find(request.CategoryId);
            request.Subject = subject.Name;

            if (request.DepartmentId == null)
            {
                request.ExecutorId = null;
                request.DepartmentId = user.DepartmentId;
            }
            DateTime current = DateTime.Now;
            var LifecycleId = request.Id;
            if (request.ExecutorId == null)
            {
                request.Status = (int)RequestStatus.Opened;
                Lifecycle newLifecycle = new Lifecycle() { Opened = current, Id = LifecycleId };
                request.LifecycleId = LifecycleId;
                request.Lifecycle = newLifecycle;
                db.Lifecycles.Add(newLifecycle);
            }
            else
            {
                request.Status = (int)RequestStatus.Proccesing;
                Lifecycle newLifecycle = new Lifecycle() { Opened = current, Proccesing = current, Id = LifecycleId };
                request.LifecycleId = LifecycleId;
                request.Lifecycle = newLifecycle;
                db.Lifecycles.Add(newLifecycle);
            }

            // если получен файл
            if (error != null)
            {
                //Получаем расширение
                string ext = error.FileName.Substring(error.FileName.LastIndexOf('.'));
                //сохраняем файл по определенному пути на сервере
                string path = current.ToString("dd.mm.yyyy hh:mm").Replace(":", "_").Replace("/", ".") + ext;
                error.SaveAs(Server.MapPath("~/Files/" + path));
                request.File = path;
            }
  

            db.AppTasks.Add(request);
            db.SaveChanges();

            return RedirectToAction("AllTasks");
        }

        [HttpGet]
        [Authorize(Roles = "Пользователь")]
        public ActionResult archDistrict_Tasks(int? page)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            var role = Roles.GetRolesForUser(user.Login).GetValue(0).ToString();
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var requests = db.AppTasks.Where(r => r.CreateDistrictId == user.DistrictId)
                .Include(r => r.Category) 
                .Include(r => r.Lifecycle) 
                .Include(r => r.Executor) 
                .Where(r => r.Status == 4)
                .OrderByDescending(r => r.Lifecycle.Opened);
            
            return View("DistrictTasks",requests.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult arch_Tasks(int? page)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            var role = Roles.GetRolesForUser(user.Login).GetValue(0).ToString();
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var requests = db.AppTasks.Where(r => r.ExecutorId == user.Id)
                .Include(r => r.Category)
                .Include(r => r.Lifecycle)
                .Include(r => r.Executor) 
                .Where(r => r.Status == 4)
                .OrderByDescending(r => r.Lifecycle.Closed);

            return View("MyTasks", requests.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        [Authorize(Roles = "Пользователь")]
        public ActionResult inprogressDistrict_Tasks(int? page)
        {
            User user = db.Users.First(m => m.Login == HttpContext.User.Identity.Name);
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var requests = db.AppTasks.Where(r => r.CreateDistrictId == user.DistrictId)
                .Where(r=>r.ExecutorId != null)
                .Include(r => r.Category)
                .Include(r => r.Lifecycle)
                .Include(r => r.Executor)
                .Where(r => r.Status == 2 || r.Status == 3)
                .OrderByDescending(r => r.Lifecycle.Opened); 
            
            return View("DistrictTasks", requests.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        [Authorize(Roles = "Модератор")]
        public ActionResult inprogress_Tasks(int? page)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            var role = Roles.GetRolesForUser(user.Login).GetValue(0).ToString();
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var requests = db.AppTasks.Where(r => r.ExecutorId == user.Id)
                .Include(r => r.Category)
                .Include(r => r.Lifecycle)
                .Include(r => r.Executor)
                .Where(r => r.Status == 2 || r.Status == 3)
                .OrderByDescending(r => r.Lifecycle.Proccesing); 

            return View("MyTasks", requests.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult inprogressMod_Tasks(int? page)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            var role = Roles.GetRolesForUser(user.Login).GetValue(0).ToString();
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var requests = db.AppTasks.Where(r => r.DepartmentId == user.DepartmentId)
                .Include(r => r.Category)
                .Include(r => r.Lifecycle)
                .Include(r => r.Executor) 
                .Where(r => r.Status == 2)
                .OrderByDescending(r => r.Lifecycle.Proccesing);

            return View("MyTasks", requests.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult inprogressUser_Tasks(int? page)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            var role = Roles.GetRolesForUser(user.Login).GetValue(0).ToString();
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var requests = db.AppTasks.Where(r => r.CreateDistrictId == user.Id)
                .Include(r => r.Category)
                .Include(r => r.Lifecycle)
                .Include(r => r.Executor)
                .Where(r => r.Status == 2)
                .OrderByDescending(r => r.Lifecycle.Proccesing); 

            return View("MyTasks", requests.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult outgoing_Tasks(int? page)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            var role = Roles.GetRolesForUser(user.Login).GetValue(0).ToString();
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var requests = db.AppTasks.Where(r => r.ExecutorId == user.Id)
                .Include(r => r.Category)
                .Include(r => r.Lifecycle)
                .Include(r => r.Executor)
                .Where(r => r.Status != 4)
                .OrderByDescending(r => r.Lifecycle.Proccesing);

            return View("MyTasks", requests.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult outgoingMod_Tasks(int? page)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            var role = Roles.GetRolesForUser(user.Login).GetValue(0).ToString();
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var requests = db.AppTasks.Where(r => r.DepartmentId == user.DepartmentId)
                .Include(r => r.Category) 
                .Include(r => r.Lifecycle)
                .Include(r => r.Executor)
                .Where(r => r.Status != 4)
                .OrderByDescending(r => r.Lifecycle.Proccesing); 

            return View("MyTasks", requests.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult outgoingUser_Tasks(int? page)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            var role = Roles.GetRolesForUser(user.Login).GetValue(0).ToString();
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var requests = db.AppTasks.Where(r => r.CreateDistrictId == user.DistrictId)
                .Include(r => r.Category)
                .Include(r => r.Lifecycle)
                .Include(r => r.Executor)
                .Where(r => r.Status == 1)
                .OrderByDescending(r => r.Lifecycle.Opened);

            return View("MyTasks", requests.ToPagedList(pageNumber, pageSize));

        }

        [HttpGet]
        public ActionResult open_Tasks(int? page)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            var role = Roles.GetRolesForUser(user.Login).GetValue(0).ToString();
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var requests = db.AppTasks.Where(r => r.DepartmentId == user.DepartmentId)
                .Include(r => r.Category)
                .Include(r => r.Lifecycle)
                .Include(r => r.Executor)
                .Where(r => r.Status == 1)
                .OrderByDescending(r => r.Lifecycle.Opened); 

            return View("AllTasks", requests.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult openMod_Tasks(int? page)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            var role = Roles.GetRolesForUser(user.Login).GetValue(0).ToString();
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var requests = db.AppTasks.Where(r => r.DepartmentId == user.DepartmentId)
                .Include(r => r.Category)
                .Include(r => r.Lifecycle)
                .Include(r => r.Executor)
                .Where(r => r.Status == 1)
                .Where(r=>r.ExecutorId == null)
                .OrderByDescending(r => r.Lifecycle.Opened); 

            return View("AllTasks", requests.ToPagedList(pageNumber, pageSize));
        }


        [HttpGet]
        public ActionResult checkingUser_Tasks(int? page)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            var role = Roles.GetRolesForUser(user.Login).GetValue(0).ToString();
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var requests = db.AppTasks.Where(r => r.CreateDistrictId == user.DistrictId)
                .Include(r => r.Category)
                .Include(r => r.Lifecycle)
                .Include(r => r.Executor) 
                .Where(r => r.Status == 3)
                .OrderByDescending(r => r.Lifecycle.Checking);

            return View("MyTasks", requests.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult checking_Tasks(int? page)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            var role = Roles.GetRolesForUser(user.Login).GetValue(0).ToString();
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var requests = db.AppTasks.Where(r => r.ExecutorId == user.Id)
                .Include(r => r.Category)
                .Include(r => r.Lifecycle)
                .Include(r => r.Executor)
                .Where(r => r.Status == 3)
                .OrderByDescending(r => r.Lifecycle.Checking);

            return View("MyTasks", requests.ToPagedList(pageNumber, pageSize));
        }


        // Создание новой заявки
        [HttpPost]
        [MultiButton(MatchFormKey="action",MatchFormValue = "enter_districtTask")]
        public  ActionResult NewTask(AppTask request,HttpPostedFileBase error) {

            var tasks = 0;

            if (!db.AppTasks.Select(r => r.Number).Any())
            {
                tasks = 1;
            }
            else
            {
                tasks = db.AppTasks.Select(r => r.Number).Max() + 1;
            }

            User user = db.Users.FirstOrDefault(m => m.Login == HttpContext.User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("LogOff", "Account");
            }

                request.Id = Guid.NewGuid();
                request.Number = tasks;
                District district = db.Districts.Find(Guid.Parse(request.Office));
                request.District = district.DistrictName;
                request.Office = district.OfficeName;
                var requestId = request.Id;
                request.CreateUserId = user.Id;
                var executorId = user.Id;
                request.Priority = Convert.ToInt32(Request.Form.GetValues("prio").FirstOrDefault());
                request.DepartmentId = db.Users.Find(request.ExecutorId)?.DepartmentId;

            var subject = db.Categorys.Find(request.CategoryId);
            request.Subject = subject.Name;

            if (request.DepartmentId == null)
                {
                    request.ExecutorId = null;
                    request.DepartmentId = user.DepartmentId;
                }

                DateTime current = DateTime.Now;
                var LifecycleId = request.Id;

                if (request.ExecutorId == null)
                {
                    request.Status = (int)RequestStatus.Opened;
                    Lifecycle newLifecycle = new Lifecycle() { Opened = current, Id = LifecycleId };
                    request.LifecycleId = LifecycleId;
                    request.Lifecycle = newLifecycle;
                    db.Lifecycles.Add(newLifecycle);
                }
                else
                {
                    request.Status = (int)RequestStatus.Proccesing;
                    Lifecycle newLifecycle = new Lifecycle() { Proccesing = current, Id = LifecycleId };
                    request.LifecycleId = LifecycleId;
                    request.Lifecycle = newLifecycle;

                    db.Lifecycles.Add(newLifecycle);
                }

                // если получен файл
                if (error != null)
                {
                    //Получаем расширение
                    string ext = error.FileName.Substring(error.FileName.LastIndexOf('.'));
                    //сохраняем файл по определенному пути на сервере
                    string path = current.ToString("dd.mm.yyyy hh:mm").Replace(":", "_").Replace("/", ".") + ext;
                    error.SaveAs(Server.MapPath("~/Files/" + path));
                    request.File = path;
                }


                db.AppTasks.Add(request);
                db.SaveChanges();

                //SendMessage("Добавлен новый объект");
                return RedirectToAction("AllTasks");
        }


        [HttpPost]
        [Authorize(Roles = "Пользователь")]
        public ActionResult DistrictNewTask(AppTask request, HttpPostedFileBase error)
        {
            var tasks = 0;

            if (db.AppTasks.Select(r => r.Number).Count() == 0)
            {
                tasks = 1;
            }
            else
            {
                tasks = db.AppTasks.Select(r => r.Number).Max() + 1;
            }
            
            User user = db.Users.FirstOrDefault(m => m.Login == HttpContext.User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("LogOff", "Account");
            }
    
            request.Id = Guid.NewGuid();
            request.Number = tasks;
            District district = db.Districts.Find(Guid.Parse(request.Office));
            request.District = district.DistrictName;
            request.Office = district.OfficeName;
            request.Category = request.Category;
            request.Comment = request.Comment;
            request.DistrictId = request.DistrictId;
            request.CreateDistrictId = user.DistrictId;   
            request.ExecutorId = null;
            request.DepartmentId = request.DepartmentId;
            request.Priority = 1;
            
            var subject = db.Categorys.Find(request.CategoryId);
            request.Subject = subject.Name;
            

            DateTime current = DateTime.Now;
            var LifecycleId = request.Id;
            
                request.Status = (int)RequestStatus.Opened;
 
                Lifecycle newLifecycle = new Lifecycle() { Opened = current, Id = LifecycleId };
                request.LifecycleId = LifecycleId;
                request.Lifecycle = newLifecycle;

                db.Lifecycles.Add(newLifecycle);
            
                if (error != null)
                {
                    //Получаем расширение
                    string ext = error.FileName.Substring(error.FileName.LastIndexOf('.'));
                    //сохраняем файл по определенному пути на сервере
                    string path = current.ToString("dd.mm.yyyy hh:mm").Replace(":", "_").Replace("/", ".") + ext;
                    error.SaveAs(Server.MapPath("~/Files/" + path));
                    request.File = path;
                }
           
            //Добавляем заявку
            db.AppTasks.Add(request);
            db.SaveChanges();

            //SendMessage("Добавлен новый объект");
            return RedirectToAction("DistrictMain");
        }

        [HttpPost]
        public ActionResult Assign(Guid requestId)
        {
            AppTask request = db.AppTasks.Find(requestId);
            User user = db.Users.First(m => m.Login == HttpContext.User.Identity.Name);
            if (request != null)
            {
                request.ExecutorId = user.Id;
                request.Status = 2;
                
                Lifecycle lifecycle = db.Lifecycles.Find(request.LifecycleId);

                lifecycle.Proccesing = DateTime.Now;

                db.Entry(lifecycle).State = EntityState.Modified;
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                
            }
            return RedirectToAction("MyTasks");
        }

        // загружаем файл
        public ActionResult Download(Guid id)
        {

            AppTask r = db.AppTasks.Find(id);

            if (r != null)
            {
                string filename = Server.MapPath("~/Files/" + r.File);
                string contentType = "image/jpeg";

              string ext = filename.Substring(filename.LastIndexOf('.'));
                switch (ext)
                {
                    case "txt":
                        contentType = "text/plain";
                        break;
                    case "png":
                        contentType = "image/png";
                        break;
                    case "tiff":
                        contentType = "image/tiff";
                        break;
                }
                return File(filename, contentType, filename);
            }
            return Content("Файл не найден");
        }

          private void SendMessage(string message)
        {
            // Получаем контекст хаба
            var context = 
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            
            // отправляем сообщение
            context.Clients.All.displayMessage(message);
        }
        [HttpGet]
        public ActionResult TaskView(Guid id) {
           
            User user = db.Users.FirstOrDefault(m => m.Login == HttpContext.User.Identity.Name);

            var executors = db.Users.Where(r => r.DepartmentId == user.DepartmentId).Where(r => r.Role.Name.ToString() != "Модератор");
            ViewBag.Executors = new SelectList(executors, "Id", "Name");
            AppTask task = db.AppTasks.Find(id);
     
            return View(task);
        }

        //представление для просмотра заявок текущего пользователя
        [HttpGet]
        [Authorize(Roles = "Исполнитель, Модератор")]
        public ActionResult MyTasks(int? page)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            var role = Roles.GetRolesForUser(user.Login).GetValue(0).ToString();
            int pageNumber = page ?? 1;
            int pageSize = 10;

            if (role == "Исполнитель")
            {
                var requests = db.AppTasks.Where(r => r.ExecutorId == user.Id)
                    .Include(r => r.Category)
                    .Include(r => r.Lifecycle)
                    .Include(r => r.Executor)
                    .Where(r => r.Status != 4)
                    .OrderByDescending(r => r.Lifecycle.Opened);

                return View(requests.ToPagedList(pageNumber, pageSize));

            }
            else
            {
                var requests = db.AppTasks.Where(r => r.ExecutorId == user.Id)
                    .Include(r => r.Category) 
                    .Include(r => r.Lifecycle)
                    .Include(r => r.Executor) 
                    .Where(r => r.Status != 4)
                    .OrderByDescending(r => r.Lifecycle.Opened);   

             return View(requests.ToPagedList(pageNumber, pageSize));
            }
        }

        [HttpGet]
        [Authorize(Roles = "Пользователь")]
        public ActionResult DistrictTasks(int? page)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
        
            int pageNumber = page ?? 1;
            int pageSize = 10;

                var requests = db.AppTasks.Where(r => r.CreateDistrictId == user.DistrictId)
                    .Include(r => r.Category) 
                    .Include(r => r.Lifecycle) 
                    .Include(r => r.Executor) 
                    .Where(r => r.Status != 4)
                    .OrderByDescending(r => r.Lifecycle.Opened); 
  

            return View(requests.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        [Authorize(Roles = "Администратор, Исполнитель, Модератор")]
        public ActionResult AllTasks(int? page)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            var role = Roles.GetRolesForUser(user.Login).GetValue(0).ToString();
            int pageNumber = page ?? 1;
            int pageSize = 10;

            if (role == "Исполнитель")
            {
                var requests = db.AppTasks.Include(r => r.Category)
                    .Include(r => r.Lifecycle) 
                    .Include(r => r.Executor) 
                    .OrderBy(r => r.Lifecycle.Opened) 
                    .Where(r => r.Status == (int)RequestStatus.Opened)
                    .Where(r => r.ExecutorId==null)
                    .Where(r => r.DepartmentId == user.DepartmentId);

                return View(requests.ToPagedList(pageNumber, pageSize));
            }
            else if (role == "Модератор")
            {

                var requests = db.AppTasks.Include(r => r.Category) 
                    .Include(r => r.Lifecycle) 
                    .Include(r => r.Executor) 
                    .OrderByDescending(r => r.Lifecycle.Opened)
                    .Where(r => r.Status != 4)
                    .Where(r => r.DepartmentId == user.DepartmentId);

                return View(requests.ToPagedList(pageNumber, pageSize));
            }
            else if (role == "Администратор")
            {
                var requests = db.AppTasks.Include(r => r.Category) 
                   .Include(r => r.Lifecycle) 
                   .Include(r => r.Executor) 
                   .OrderByDescending(r => r.Lifecycle.Opened);

                return View(requests.ToPagedList(pageNumber, pageSize));
            }
            return RedirectToAction("LogOff", "Account");
        }

        public ActionResult HelpInfo()
        {
            return View("HelpInfo");
        }
        public ActionResult Main()
        {
          return View("Main");
        }

        public ActionResult DistrictMain()
        {
            return View("DistrictMain");
        }

        [HttpGet]
        public ActionResult NewTask()
        {
            User user = db.Users.FirstOrDefault(m => m.Login == HttpContext.User.Identity.Name);
            Role role = db.Roles.FirstOrDefault(r => r.Id == user.RoleId);
            if (user != null)
            {
                var districts = db.Districts.GroupBy(d => d.DistrictName)
                    .Select(di => di.FirstOrDefault())
                    .ToList();
                var categories = db.Categorys
                    .Where(c => c.DepartmentId == user.DepartmentId)
                    .ToList();

                    string selectedIndex = "г. Калуга"; //db.Districts.Select(d=>d.DistrictName).First();

                    ViewBag.Districts = new SelectList(districts, "Id", "DistrictName", selectedIndex);

                    ViewBag.Offices = new SelectList(db.Districts.Where(c => c.DistrictName == selectedIndex), "Id", "OfficeName");
                    ViewBag.Categories = new SelectList(categories, "Id", "Name");
                    Department dept = db.Departments.FirstOrDefault(d => d.Id == user.DepartmentId);
                    ViewBag.Users = new SelectList(db.Users.Where(d => d.DepartmentId == dept.Id).Where(r=>r.Role.Name.ToString()!="Модератор"), "Id", "Name");

                return View();
            }
            return RedirectToAction("LogOff", "Account");
        }

        [HttpGet]
        [Authorize(Roles = "Пользователь")]
        public ActionResult DistrictNewTask()
        {
            User user = db.Users.FirstOrDefault(m => m.Login == HttpContext.User.Identity.Name);
            Role role = db.Roles.FirstOrDefault(r => r.Id == user.RoleId);
            if (user != null)
            {
                var districts = db.Districts.GroupBy(d => d.DistrictName)
                    .Select(di => di.FirstOrDefault())
                    .ToList();
                
                var departments = db.Departments.ToList();

                 
                    ViewBag.Districts = new SelectList(districts, "Id", "DistrictName");
                    ViewBag.Offices = new SelectList(db.Districts, "Id", "OfficeName");

                    ViewBag.Departments = new SelectList(departments, "Id", "Name");
                    ViewBag.Categories = new SelectList(db.Categorys, "Id", "Name");

                return View();
            }
            return RedirectToAction("LogOff", "Account");
        }

        [HttpPost]
        public ActionResult Change_Executor(Guid requestId) {
            
            var executorName = Request.Form.GetValues("unit_id").FirstOrDefault();
            User executor = db.Users.FirstOrDefault(r => r.Name== executorName);
            AppTask req = db.AppTasks.Find(requestId);
          
            req.ExecutorId = executor.Id;
            req.Status = (int)RequestStatus.Proccesing;
            Lifecycle lifecycle = db.Lifecycles.Find(req.LifecycleId);
            lifecycle.Proccesing = DateTime.Now;
            db.Entry(lifecycle).State = EntityState.Modified;

            db.Entry(req).State = EntityState.Modified;
            db.SaveChanges();
          return View("Main");
        }

        [HttpPost]
        public ActionResult Change_Priority(Guid requestId)
        {
            var priority = Request.Form.GetValues("priority").FirstOrDefault();
            AppTask req = db.AppTasks.Find(requestId);

            Lifecycle lifecycle = db.Lifecycles.Find(req.LifecycleId);
            lifecycle.Proccesing = DateTime.Now;
            req.Priority = Convert.ToInt32(priority);
            db.Entry(lifecycle).State = EntityState.Modified;

            db.Entry(req).State = EntityState.Modified;
            db.SaveChanges();
            return View("Main");
        }

        [HttpPost]
        public ActionResult EndTask(Guid requestId)

        {
            User user = db.Users.FirstOrDefault(m => m.Login == User.Identity.Name);

            AppTask req = db.AppTasks.Find(requestId);

            if (req.ExecutorId != user.Id) {
                req.Status = (int)RequestStatus.Checking;
                Lifecycle lifecycle = db.Lifecycles.Find(req.LifecycleId);
                lifecycle.Checking = DateTime.Now;
                db.Entry(lifecycle).State = EntityState.Modified;

                db.Entry(req).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                req.Status = (int)RequestStatus.Checking;
                Lifecycle lifecycle = db.Lifecycles.Find(req.LifecycleId);
                lifecycle.Checking = DateTime.Now;
                db.Entry(lifecycle).State = EntityState.Modified;

                db.Entry(req).State = EntityState.Modified;
                db.SaveChanges();
            }

            return View("Main");
        }

        [HttpPost]
        public ActionResult FinishEndTask(Guid requestId)

        {
            AppTask req = db.AppTasks.Find(requestId);

            req.Status = (int)RequestStatus.Closed;
            Lifecycle lifecycle = db.Lifecycles.Find(req.LifecycleId);
            lifecycle.Closed = DateTime.Now;
            db.Entry(lifecycle).State = EntityState.Modified;

            db.Entry(req).State = EntityState.Modified;
            db.SaveChanges();
            return View("DistrictMain");
        }
        
        public ActionResult AddComment(Comment comment, Guid taskId)
        {
            
            // получаем текущего пользователя
            User user = db.Users.FirstOrDefault(m => m.Login == HttpContext.User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("LogOff", "Account");
            }

            comment.Id = Guid.NewGuid();
            comment.UserId = user.Id;
            comment.TaskId = taskId;
            comment.Text = comment.Text;
            comment.Date = DateTime.Now;
           
            db.Comments.Add(comment);
            db.SaveChanges();

            SendMessage("Добавлен новый комментарий");
            return RedirectToRoute(Request.Url);

        }


    }
}
