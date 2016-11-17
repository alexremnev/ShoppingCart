using System;

namespace ShoppingCart.DAL
{
    public abstract class BaseEntity
    {
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
        public virtual string Creator { get; set; }
    }
}
