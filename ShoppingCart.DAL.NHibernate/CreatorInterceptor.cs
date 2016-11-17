using NHibernate;
using NHibernate.Type;

namespace ShoppingCart.DAL.NHibernate
{
    public class CreatorInterceptor : EmptyInterceptor
    {
        private readonly ISecurityContext _context;

        public CreatorInterceptor(ISecurityContext context)
        {
            _context = context;
        }
        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            for (var i = 0; i < propertyNames.Length; i++)
            {
                if (propertyNames[i] == "Creator")
                {
                    state[i] = _context.UserName;
                    return true;
                }
            }
            return false;
        }

    }
}
