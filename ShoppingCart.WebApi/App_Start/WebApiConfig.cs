using System.Web.Http;

namespace ShoppingCart.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
       name: "CountRoute",
       routeTemplate: "api/{controller}/count/{filter}",
       defaults: new { filter = "" }
   );

            config.Routes.MapHttpRoute(
     name: "DefaultApi",
     routeTemplate: "api/{controller}/{id}",
     defaults: new { id = RouteParameter.Optional },
     constraints: new { id = @"\d+" }
);

            config.Routes.MapHttpRoute(
       name: "FiveParamRoute",
       routeTemplate: "api/{controller}/{filter}/{sortby}/{sortDirection}/{page}/{pageSize}",
       defaults: new { filter = "", sortby = "", sortDirection = true, page = 1, pageSize = 50 }
 );

        }
    }
}
