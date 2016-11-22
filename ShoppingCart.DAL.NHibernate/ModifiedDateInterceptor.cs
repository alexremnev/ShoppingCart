using System;
using NHibernate.Type;

namespace ShoppingCart.DAL.NHibernate
{
    public class ModifiedDateInterceptor : BaseInterceptor<DateTime>
    {
        private const string Property = "ModifiedDate";
       
        public ModifiedDateInterceptor() : base(Property, DateTime.Now) { }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] prevousState, string[] propertyNames,
                  IType[] types)
        {
            return SetValue(currentState, propertyNames, DateTime.Now);
        }
    }
}
