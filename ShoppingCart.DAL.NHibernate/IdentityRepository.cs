using System;
using System.Linq;
using Common.Logging;
using NHibernate;

namespace ShoppingCart.DAL.NHibernate
{
    public class IdentityRepository : BaseRepository<Identity>, IIdentityRepository

    {
        private static readonly ILog Log = LogManager.GetLogger<IdentityRepository>();
        private const string NameEntity = "identity";

        public IdentityRepository() : base(Log, NameEntity) { }

        public Identity FindIdentity(string name, string password)
        {
            Func<IQueryOver<Identity, Identity>, IQueryOver<Identity, Identity>> applyFilter =
                delegate (IQueryOver<Identity, Identity> query)
                {
                    query.And(x => x.UserName == name);
                    query.And(x => x.Password == password);
                    return query;
                };
            var users = List(0, 50, applyFilter);
            return users.FirstOrDefault();
            //return users != null ? users[0] : null;
        }
    }
}
