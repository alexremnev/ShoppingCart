namespace ShoppingCart.DAL
{
    public class LineItem : BaseEntity, IJsonObject
    {
        public virtual int Id { get; set; }
        public virtual int ProductId { get; set; }
        public virtual string Name { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal Price { get; set; }
    }
}
