using NHibernate;
using NHibernate.Type;

namespace ShoppingCart.DAL.NHibernate
{
    public abstract class BaseInterceptor<T> : EmptyInterceptor
    {
        private readonly string _property;
        private readonly T _value;

        protected BaseInterceptor(string property, T value)
        {
            _property = property;
            _value = value;
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            return SetValue(state, propertyNames, _value);
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
