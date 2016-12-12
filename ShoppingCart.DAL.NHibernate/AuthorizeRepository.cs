using Common.Logging;

namespace ShoppingCart.DAL.NHibernate
{
    public class AuthorizeRepository : BaseRepository<ControllerName>, IAuthorizeRepository
    {
        private static readonly ILog Log = LogManager.GetLogger<AuthorizeRepository>();
        private const string NameEntity = "role";

        public AuthorizeRepository() : base(Log, NameEntity) { }

    }
}
