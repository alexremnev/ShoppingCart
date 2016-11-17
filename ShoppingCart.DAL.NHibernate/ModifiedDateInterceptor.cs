using System;
using NHibernate;
using NHibernate.Type;

namespace ShoppingCart.DAL.NHibernate
{
    public class ModifiedDateInterceptor : EmptyInterceptor
    {
        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            for (var i = 0; i < propertyNames.Length; i++)
            {
                if (propertyNames[i] != "ModifiedDate") continue;
                state[i] = DateTime.Now;
                return true;
            }
            return false;
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] prevousState, string[] propertyNames,
            IType[] types)
        {
            for (var i = 0; i < propertyNames.Length; i++)
            {
                if (propertyNames[i] != "ModifiedDate") continue;
                currentState[i] = DateTime.Now;
                return true;
            }
            return false;
        }

    }

}
