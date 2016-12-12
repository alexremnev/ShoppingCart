using System.Web.Mvc;
using System.Web.Routing;

namespace ShoppingCart.WebApi_2.App_Start
{

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Customer", action = "Get", id = UrlParameter.Optional },
                namespaces: new[] { "ShoppingCart.WebApi_2.Controllers" }
            );
        }
    }

}