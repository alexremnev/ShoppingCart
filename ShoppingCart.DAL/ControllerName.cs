using System.Collections.Generic;

namespace ShoppingCart.DAL
{
    public class ControllerName : BaseEntity
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<Method> Methods { get; set; }
    }
}
