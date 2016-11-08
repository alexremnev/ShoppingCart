using System.Collections.Generic;

namespace ShoppingCart.DAL
{
    public class Order
    {
        public virtual int Id { get; set; }
        public virtual decimal Total { get; set; }
        public virtual IList<LineItem> LineItems { get; set; }
    }
}
