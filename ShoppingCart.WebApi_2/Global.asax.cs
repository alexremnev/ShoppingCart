using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;


namespace ShoppingCart.WebApi_2
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
         //  GlobalConfiguration.Configure(WebApiConfig.Register);
            var controllerFactory = new ControllerFactory();
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
            //GlobalConfiguration.Configuration.DependencyResolver = new MyDependencyResolver();
            //ServiceLocator.RegisterService<IProductService>(typeof(ProductService));
            //var container = new Container();
            //container.Register<IProductService>(c => new ProductService());
            //DependencyResolver.SetResolver(new MyDependencyResolver(container));
        }

    }
}
