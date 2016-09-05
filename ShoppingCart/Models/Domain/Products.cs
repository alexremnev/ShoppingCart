using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCart.Models.Domain
{
    public class Products
    {
        public virtual Guid Id { get; set;}
        public virtual string Name { get; set; }
        public virtual int Quantity { get; set; }
        public virtual double Price { get; set; }

        
    }
}