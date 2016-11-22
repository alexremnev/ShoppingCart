using NHibernate.Type;

namespace ShoppingCart.DAL.NHibernate
{
    public class CreatorInterceptor : BaseInterceptor<string>
    {
        private const string Property = "Creator";
        private readonly ISecurityContext _context;

        public CreatorInterceptor(ISecurityContext context) : base(Property, null)
        {
            _context = context;
        }
        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (entity is IChangeableEntity) return SetValue(state, propertyNames, _context.UserName);
            return false;
            
        }
    }
}
