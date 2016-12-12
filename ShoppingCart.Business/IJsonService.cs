using System.Collections.Generic;
using ShoppingCart.DAL;

namespace ShoppingCart.Business
{
    public interface IJsonService
    {
        string Serialize(object value);
        IList<Discount> Desirialize(string value);
    }
}
