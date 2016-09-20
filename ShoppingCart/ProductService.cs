using System.Collections.Generic;
using ShoppingCart.Models;
using ShoppingCart.Models.Domain;

namespace ShoppingCart
{
    public class ProductService : IProductService
    {
        public IProductRepository Repo { get; set; }

        public IList<Product> List()
        {
            return Repo.List();
        }

        public Product Get(int id)
        {
            var product = Repo.Get(id);
            return product;
        }
    }
}