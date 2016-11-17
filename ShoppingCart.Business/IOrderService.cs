using System.Collections.Generic;
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
        /// <summary>
        /// Update an order.
        /// </summary>
        /// <param name="entity">entity order.</param>
        void Update(Order entity);
        /// <summary>
        /// Return quantity products in database
        /// </summary>
        /// <param name="products">list of products.</param>
        void ReturnProducts(IList<LineItem> products);
    }
}
