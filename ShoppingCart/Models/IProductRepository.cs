using System.Collections.Generic;
using ShoppingCart.Models.Domain;

namespace ShoppingCart.Models
{
    public interface IProductRepository
    {
        void Create(Product newProducts);
        Product FindById(int id);
        void Delete(Product product);
        IList<Product> Read();
    }
}