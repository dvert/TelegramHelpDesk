using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using TelegramHelpDesk.Models;

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

                        var role = Roles.GetRolesForUser(model.UserName).GetValue(0).ToString();

                        if (role == "Пользователь") {
                            return RedirectToAction("DistrictMain", "AppTask");
                        }
                        else if (role == "Модератор" || role == "Исполнитель" || role == "Администратор")
                        {
                            return RedirectToAction("Main", "AppTask");
                        }
                        else {
                            return Redirect(returnUrl);
                        }
                    }
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
