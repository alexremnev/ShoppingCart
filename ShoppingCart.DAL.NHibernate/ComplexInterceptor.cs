using System.Collections.Generic;
using NHibernate;
using NHibernate.Type;

namespace ShoppingCart.DAL.NHibernate
{
    public class ComplexInterceptor : EmptyInterceptor
    {
        private readonly List<IInterceptor> _interceptors = new List<IInterceptor>();

        public ComplexInterceptor(IInterceptor creatorInterceptor, IInterceptor modifiedInterceptor, IInterceptor createdInterceptor)
        {
            _interceptors.Add(creatorInterceptor);
            _interceptors.Add(modifiedInterceptor);
            _interceptors.Add(createdInterceptor);
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
          
            foreach (var interceptor in _interceptors)
            {
                interceptor.OnSave(entity, id, state, propertyNames, types);
            }

            return true;
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames,
            IType[] types)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
            }
            return true;
        }
    }
}
