using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using Spring.Web.Mvc;

namespace ShoppingCart.WebApi
{
    public class Global : SpringMvcApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());
        }
    }
}