﻿using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace ShoppingCart.DAL.NHibernate
{
    public class NhibernateHelper
    {
        private static ISessionFactory _sessionfactory;

        private static ISessionFactory Sessionfactory
        {
            get
            {
                if (_sessionfactory != null) return _sessionfactory;
                var configuration = new Configuration();
                configuration.Configure();
                configuration.AddAssembly(typeof(Product).Assembly);
                _sessionfactory = configuration.BuildSessionFactory();
                return _sessionfactory;

            }
        }
        public static ISession OpenSession()
        {
            return Sessionfactory.OpenSession();
        }
    }
}