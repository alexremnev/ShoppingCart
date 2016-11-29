using System;
using NHibernate.Type;

namespace ShoppingCart.DAL.NHibernate
{
    public class CreatedDateInterceptor : BaseInterceptor<DateTime>
    {
        private const string Property = "CreatedDate";
        public CreatedDateInterceptor() : base(Property) { }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
           return SetValue(state, propertyNames, DateTime.Now);
        }
    }
}
