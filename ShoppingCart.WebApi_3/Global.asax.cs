using System.Web.Mvc;
using System.Web.Routing;


namespace ShoppingCart.WebApi_3
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
        
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}

