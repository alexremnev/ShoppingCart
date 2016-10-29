using System.Web.Http;

namespace ShoppingCart.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
     "DefaultApi",
     "api/{controller}/{id}",
     new { id = RouteParameter.Optional },
     new { id = @"\d+" }
);
            config.Routes.MapHttpRoute(
     "ListRoute",
     "api/{controller}/{action}"
);
        }
    }
}
