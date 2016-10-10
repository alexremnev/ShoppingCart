using System.Collections.Generic;
using ShoppingCart.DAL;

namespace ShoppingCart
{
    public class ProductService : IProductService
    {
        public IProductRepository Repo { get; set; }

        public IList<Product> List(string filter, string sortby, bool sortDirection, int firstResult, int maxResults)
        {
            return Repo.List(filter, sortby, sortDirection, firstResult, maxResults);
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

        public int Count(string filter)
        {
            return Repo.Count(filter);
        }
    }
}