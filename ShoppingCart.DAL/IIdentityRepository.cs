namespace ShoppingCart.DAL
{
    public interface IIdentityRepository : IRepository<Identity>
    {
        Identity FindIdentity(string name, string password);
    }
}
