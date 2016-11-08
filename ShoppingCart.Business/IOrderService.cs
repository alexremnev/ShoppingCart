using ShoppingCart.DAL;

namespace ShoppingCart.Business
{
    public interface IOrderService : IService<Order>
    {
        /// <summary>
        /// Place an Order.
        /// </summary>
        /// <param name="entity">list of lineItems. If entity is null occurs RepositoryException.</param>
        void Place(Order entity);
    }
}
