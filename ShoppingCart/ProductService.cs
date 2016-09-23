using System.Collections.Generic;
using ShoppingCart.Models;
using ShoppingCart.Models.Domain;

namespace ShoppingCart
{
    public class ProductService : IProductService
    {
        public IProductRepository Repo { get; set; }

        public IList<Product> List(string filter, string sortby, int? maxResult, int? firstResult)
        {
            return Repo.List(filter, sortby, maxResult, firstResult);
        }

        public Product Get(int id)
        {
            var product = Repo.Get(id);
            return product;
        }

        public void Create(Product entity)
        {
            Repo.Create(entity);
        }

        public void Delete(int id)
        {
            Repo.Delete(id);
        }
    }
}