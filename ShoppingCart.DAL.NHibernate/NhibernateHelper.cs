using NHibernate;
using NHibernate.Cfg;

namespace ShoppingCart.DAL.NHibernate
{
    public static class NhibernateHelper
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

        public static void Reset()
        {
            if (_sessionfactory == null) return;
            _sessionfactory.Dispose();
            _sessionfactory = null;

        }
        public static ISession OpenSession()
        {
            return Sessionfactory.OpenSession();
        }
    }
}