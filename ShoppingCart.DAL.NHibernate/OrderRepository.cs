using Common.Logging;

namespace ShoppingCart.DAL.NHibernate
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private static readonly ILog Log = LogManager.GetLogger<OrderRepository>();
        private const string NameEntity = "order";

        public OrderRepository() : base(Log, NameEntity) { }
    }
}
