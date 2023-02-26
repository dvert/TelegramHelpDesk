using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TelegramHelpDesk.Models;
using System.Windows.Forms;

namespace TelegramHelpDesk.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Login(LogViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    //if (Url.IsLocalUrl(returnUrl)) {

                        var role = Roles.GetRolesForUser(model.UserName).GetValue(0).ToString();

                        if (role == "Пользователь") {
                            return RedirectToAction("DistrictMain", "AppTask");
                        }
                        else if (role == "Модератор")
                        {
                            return RedirectToAction("Main", "AppTask");
                        }
                        else if (role == "Исполнитель")
                        {
                            return RedirectToAction("Main", "AppTask");
                        }
                        else if (role == "Администратор")
                        {
                            return RedirectToAction("Main", "AppTask");
                        }
                        else {
                            return Redirect(returnUrl);
                        }
                    }
                    
                    //else
                    //{
                    //        return RedirectToAction("Main", "AppTask");
                    //}
                //}
                else
                {
                    ModelState.AddModelError("", "Неправильный пароль или логин");
                }
            }
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Account");
        }


        private bool ValidateUser(string login, string password) {
            bool isValid = false;
      
            using (HelpdeskContext _db = new HelpdeskContext()) {

                //Clipboard.SetText(_db.Users.Count().ToString());
                try
                {
                        User user = (from u in _db.Users
                            where u.Login == login && u.Password == password
                            select u).FirstOrDefault();

                        if (user != null) {
                            isValid = true;
                        }
                    }
                    catch {
                        isValid = false;
                }
                }
                return isValid;
        }
    }
}
