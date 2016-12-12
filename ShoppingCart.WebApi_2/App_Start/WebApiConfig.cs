using System.Web.Http;
using System.Web.Mvc;
using ShoppingCart.WebApi_2.Controllers;
using ShoppingCart.WebApi_2.Service;


namespace ShoppingCart.WebApi_2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
           
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            
            //IContainer container = new Container();
            //container.Register<IProductService>(c => new ProductService());
            //var container = new IoC();
            //container.Register<CustomerController>();
            //container.Register<IProductService,ProductService>();
            //DependencyResolver.SetResolver(new MyDependencyResolver(container));
        }
    }
}
