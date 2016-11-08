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

        public IList<Customer> List(int firstResult, int maxResults)
        {
            var list = _repo.List(firstResult, maxResults);
            if (list.Count == 0) return list;
            foreach (var customer in list)
            {
                var encryptedCard = customer.Card;
                var decodeCard = CryptoEngine.Decrypt(encryptedCard);
                customer.Card = decodeCard;
            }
            return list;
        }

        public Customer Get(int id)
        {
            var customer = _repo.Get(id);
            if (customer != null)
            {
                var encryptedCard = customer.Card;
                var decodeCard = CryptoEngine.Decrypt(encryptedCard);
                customer.Card = decodeCard;
            }
            return customer;
        }

        public void Create(Customer entity)
        {
            var encodeCard = CryptoEngine.Encrypt(entity.Card);
            entity.Card = encodeCard;
            _repo.Create(entity);
        }

        public void Update(Customer entity)
        {
            var encodeCard = CryptoEngine.Encrypt(entity.Card);
            entity.Card = encodeCard;
            _repo.Update(entity);
        }
    }
}
