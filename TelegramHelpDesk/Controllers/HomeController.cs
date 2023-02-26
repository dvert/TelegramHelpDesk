using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TelegramHelpDesk.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Страница для описания.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Страница для контактов.";

            return View();
        }


    }
}