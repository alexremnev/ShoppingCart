using System;
using System.Collections.Generic;
using Common.Logging;
using NHibernate;

namespace ShoppingCart.DAL.NHibernate
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private static readonly ILog Log = LogManager.GetLogger<OrderRepository>();
        private const string NameEntity = "order";

        public OrderRepository() : base(Log, NameEntity) { }
        public IList<Order> List(int firstResult = 0, int maxResults = 50, string username = null)
        {
            Func<IQueryOver<Order, Order>, IQueryOver<Order, Order>> applyFilter =
                delegate (IQueryOver<Order, Order> query)
                {
                   return query.And(x => x.UserName == username);
                    
                };
            return List(firstResult, maxResults, applyFilter);
        }
    }
}
