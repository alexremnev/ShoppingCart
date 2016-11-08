using Common.Logging;

namespace ShoppingCart.DAL.NHibernate
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
       private static readonly ILog Log = LogManager.GetLogger<OrderRepository>();

        protected override ILog GetLog()
        {
            return Log;
        }

        protected override string GetNameOfEntity()
        {
            return "order";
        }
    }
}
