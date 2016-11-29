namespace ShoppingCart.DAL
{
    public class Product : BaseEntity, IAuditableEntity
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal Price { get; set; }
        public virtual string Sku { get; set; }
    }
}