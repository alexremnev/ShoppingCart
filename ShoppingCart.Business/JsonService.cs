using System.Collections.Generic;
using Newtonsoft.Json;
using ShoppingCart.DAL;

namespace ShoppingCart.Business
{
    public class JsonService : IJsonService
    {
        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
        
        public IList<Discount> Desirialize(string value)
        {
            return JsonConvert.DeserializeObject<IList<Discount>>(value);
        }
    }
}
