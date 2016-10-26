using System.Collections.Generic;
using ShoppingCart.DAL;

namespace ShoppingCart.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public IList<Product> List(string filter, string sortby, bool sortDirection, int firstResult, int maxResults)
        {
            return _repo.List(filter, sortby, sortDirection, firstResult, maxResults);
        }

        public Product Get(int id)
        {
            var product = _repo.Get(id);
            return product;
        }

        public void Create(Product entity)
        {
            _repo.Create(entity);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public int Count(string filter)
        {
            return _repo.Count(filter);
        }

        public bool Update(int id, Product entity)
        {
            return _repo.Update(id,entity);
        }
    }
}