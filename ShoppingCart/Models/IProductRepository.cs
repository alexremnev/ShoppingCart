using System.Collections.Generic;
using ShoppingCart.Models.Domain;

namespace ShoppingCart.Models
{
    public interface IProductRepository
    {
        void Add(Product newProducts);
        Product FindById(int id);
        void Delete(Product product);
        List<Product> ShowAllProducts();
    }
}