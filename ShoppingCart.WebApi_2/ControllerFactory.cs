using System;
using System.CodeDom;
using System.Web.Mvc;
using System.Web.Routing;
using ShoppingCart.WebApi_2.Controllers;
using ShoppingCart.WebApi_2.Service;

namespace ShoppingCart.WebApi_2
{
    public class ControllerFactory : DefaultControllerFactory
    {
        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            //if (controllerName.ToLowerInvariant() == "product")
            //{
            //    IProductService service = new ProductService();
            //    return new ShoppingCart.WebApi_2.Controllers.CustomerController();
            //}
            return base.CreateController(requestContext, controllerName);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == typeof(CustomerController))
            {
                IProductService service = new ProductService();
                return new ShoppingCart.WebApi_2.Controllers.CustomerController(service);
            }
            return base.GetControllerInstance(requestContext, controllerType);
        }

    }
}