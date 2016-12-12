using System;
using System.Collections.Generic;

namespace ShoppingCart.DAL
{
    public class Order : BaseEntity, IAuditableEntity, IChangeable
    {
        public virtual int Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual decimal Total { get; set; }
        public virtual IList<LineItem> LineItems { get; set; }
        public virtual DateTime? SaleDate { get; set; }
        public virtual IList<Discount> Discount { get; set; }
//        public virtual IList<LineItem> JsonLineItems { get; set; }
    }
}
