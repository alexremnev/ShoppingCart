using Common.Logging;

namespace ShoppingCart.DAL.NHibernate
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        private static readonly ILog Log = LogManager.GetLogger<CustomerRepository>();

        protected override ILog GetLog()
        {
            return Log;
        }

        protected override string GetNameOfEntity()
        {
            return "customer";
        }
    }
}
