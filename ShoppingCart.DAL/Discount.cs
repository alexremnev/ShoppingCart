namespace ShoppingCart.DAL
{
    public class Discount : IJsonObject
    {
        public virtual string Name { get; set; }
        public virtual decimal Amount { get; set; }
    }
}
