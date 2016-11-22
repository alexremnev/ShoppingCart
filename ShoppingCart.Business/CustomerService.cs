using System.Collections.Generic;
using ShoppingCart.DAL;

namespace ShoppingCart.Business
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;

        public CustomerService(ICustomerRepository repo)
        {
            _repo = repo;
        }

        public IList<Customer> List(string filter, string sortby, bool isAscending, int firstResult, int maxResults)
        {
            var list = _repo.List(firstResult, maxResults);
            return list;
        }

        public Customer Get(int id)
        {
            var entity = _repo.Get(id);
            return entity;
        }

        public void Create(Customer entity)
        {
            _repo.Create(entity);
        }

        public void Update(Customer entity)
        {
            _repo.Update(entity);
        }

        public int Count(string filter, decimal maxPrice)
        {
            return _repo.Count();
        }
    }
}
