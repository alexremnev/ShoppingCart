using System;

namespace ShoppingCart.DAL
{
   public class Customer: BaseEntity
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual string Card { get; set; }
       }
}
