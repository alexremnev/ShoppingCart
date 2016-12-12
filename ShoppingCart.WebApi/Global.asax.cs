using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using ShoppingCart.Business;
using Spring.Context.Support;
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
            AddConfigObject();
        }

        private void AddConfigObject()
        {
            var t = typeof(TestService);
            var attributes = t.GetCustomAttributes(false);
            foreach (var attribute in attributes)
            {
                var type = attribute.GetType();

                if (type.Name != "TestAttribute") continue;
                var context = ContextRegistry.GetContext();
                var service = new TestService();
                context.ConfigureObject(service, "Service");
                var xmlContext = context as XmlApplicationContext;
                xmlContext?.ObjectFactory.RegisterSingleton("Service", service);
            }
        }
    }
}