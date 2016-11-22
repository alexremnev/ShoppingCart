using System;
using NHibernate.Properties;

namespace ShoppingCart.DAL
{
    public class Customer : BaseEntity, IChangeableEntity
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual string Card { get; set; }
    }

}
