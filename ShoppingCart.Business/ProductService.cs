using System.Collections.Generic;
using ShoppingCart.DAL;

namespace ShoppingCart.Business
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

        public IList<Product> List(int firstResult, int maxResults)
        {
            return _repo.List(null, null, true, firstResult, maxResults);
        }

        public Product Get(int id)
        {
            var product = _repo.Get(id);
            return product;
        }

        public void Create(Product entity)
        {
            entity.Sku = GenerateSku(entity.Name);
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

        public void Update(Product entity)
        {
            entity.Sku = GenerateSku(entity.Name);
            _repo.Update(entity);
        }

        public Product GetByName(string name)
        {
            return _repo.GetByName(name);
        }
        private static string GenerateSku(string name)
        {
            var sku = name.Replace(' ', '_');
            return "sku_" + sku;
        }
    }
}