using System;
using NHibernate;
using NHibernate.Type;

namespace ShoppingCart.DAL.NHibernate
{
    public class CreatedDateInterceptor : EmptyInterceptor
    {
        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            for (var i = 0; i < propertyNames.Length; i++)
            {
                if (propertyNames[i] != "CreatedDate") continue;
                state[i] = DateTime.Now;
                return true;
            }
            return false;
        }
    }
}
