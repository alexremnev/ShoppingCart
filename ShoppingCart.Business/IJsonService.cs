using System.Collections.Generic;
using ShoppingCart.DAL;

namespace ShoppingCart.Business
{
    public interface IJsonService
    {
        string Serialize(object value);
        IList<IJsonObject> Desirialize(string value);
    }
}
