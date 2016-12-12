namespace ShoppingCart.DAL
{
    public class Identity : BaseEntity
    {
        public virtual int Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
        public virtual string Role { get; set; }
    }
}
