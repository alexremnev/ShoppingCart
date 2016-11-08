namespace ShoppingCart.DAL
{
   public class Customer
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual long Card { get; set; }
    }
}
