using NHibernate;

namespace ShoppingCart.DAL.NHibernate
{
    public abstract class BaseInterceptor<T> : EmptyInterceptor
    {
        private readonly string _property;
       
        protected BaseInterceptor(string property)
        {
            _property = property;
       }

       protected bool SetValue(object[] state, string[] propertyNames, T value)
        {
            for (var i = 0; i < propertyNames.Length; i++)
            {
                if (propertyNames[i] != _property) continue;
                state[i] = value;
                return true;
            }
            return false;
        }
    }
}
