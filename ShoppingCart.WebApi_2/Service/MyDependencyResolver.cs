using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web.Http.Dependencies;
using System.Web.Mvc;

namespace ShoppingCart.WebApi_2.Service
{
    public class MyDependencyResolver : IDependencyResolver
    {
        private readonly IoC _container;
        public MyDependencyResolver(IoC container)
        {
            _container = container;
        }
        public object GetService(Type serviceType)
        {
            //            return ServiceLocator.Resolve<IProductService>();

            //            if (serviceType == typeof(IProductService))
            //                return new ProductService();
            //            return null;
            
            return _container.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Enumerable.Empty<object>();
        }

    }


}