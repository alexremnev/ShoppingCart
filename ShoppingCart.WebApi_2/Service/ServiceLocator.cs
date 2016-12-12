using System;
using System.Collections.Generic;

namespace ShoppingCart.WebApi_2.Service
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, Type> Services = new Dictionary<Type, Type>();

        public static void RegisterService<T>(Type service)
        {
            Services[typeof(T)] = service;
        }

        public static T Resolve<T>()
        {
            return (T)Activator.CreateInstance(Services[typeof(T)]);
        }
    }
}