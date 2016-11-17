using Common.Logging;

namespace ShoppingCart.DAL.NHibernate
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        private static readonly ILog Log = LogManager.GetLogger<CustomerRepository>();
        private const string NameEntity = "customer";

        public CustomerRepository() : base(Log, NameEntity) { }
    }
}
