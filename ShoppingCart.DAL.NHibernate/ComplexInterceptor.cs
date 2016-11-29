using System.Collections.Generic;
using NHibernate;
using NHibernate.Type;

namespace ShoppingCart.DAL.NHibernate
{
    public class ComplexInterceptor : EmptyInterceptor
    {
        private readonly IList<IInterceptor> _interceptors = new List<IInterceptor>();

        public ComplexInterceptor(params IInterceptor[] interceptors)
        {
            foreach (var interceptor in interceptors)
            {
                _interceptors.Add(interceptor);
            }
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {

            foreach (var interceptor in _interceptors)
            {
                interceptor.OnSave(entity, id, state, propertyNames, types);
            }
            return true;
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState,
            string[] propertyNames,
            IType[] types)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
            }
            return true;
        }

        public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.OnLoad(entity, id, state, propertyNames, types);
            }
            return true;
        }

        public override int[] FindDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames,
              IType[] types)
        {
            var result = new int[10];
            foreach (var interceptor in _interceptors)
            {
               result = interceptor.FindDirty(entity, id, currentState, previousState, propertyNames, types);
            }
            return result;
        }
    }
}
