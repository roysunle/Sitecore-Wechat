using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.Shell.Framework.Commands.Masters;

namespace Wechat
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "api/sitecore/{controller}/{action}/{id}",
                new { id = UrlParameter.Optional }
                
            );

            routes.MapRoute(
                "Signature",
                "Signature",
                new { controller = "Signature", action = "Signature", id = UrlParameter.Optional }
                
            );
        }
    }
}
