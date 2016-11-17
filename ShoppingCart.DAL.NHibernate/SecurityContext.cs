namespace ShoppingCart.DAL.NHibernate
{
    public class SecurityContext : ISecurityContext
    {
       public string UserName { get; set; }
    }
}
