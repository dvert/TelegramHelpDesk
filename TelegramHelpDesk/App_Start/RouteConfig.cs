﻿using System.Web.Mvc;
using System.Web.Routing;

namespace TelegramHelpDesk
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "AppTask", action = "Main", id = UrlParameter.Optional }
            );
        }
    }
}
