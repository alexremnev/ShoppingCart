using System;

namespace ShoppingCart.DAL.NHibernate
{
    public class CreatedDateInterceptor : BaseInterceptor<DateTime>
    {
        private const string Property = "CreatedDate";
        public CreatedDateInterceptor() : base(Property, DateTime.Now) { }
    }
}
