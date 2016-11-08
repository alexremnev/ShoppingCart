using ShoppingCart.DAL;

namespace ShoppingCart.Business
{
    public interface ICustomerService : IService<Customer>
    {
        /// <summary>
        /// Create a customer.
        /// </summary>
        /// <param name="entity">entity customer.</param>
        void Create(Customer entity);
        /// <summary>
        /// Update a customer.
        /// </summary>
        /// <param name="entity">entity customer.</param>
        void Update(Customer entity);
    }
}
