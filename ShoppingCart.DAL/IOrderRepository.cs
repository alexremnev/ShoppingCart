using System.Collections.Generic;

namespace ShoppingCart.DAL
{
    public interface IOrderRepository : IRepository<Order>
    {
        /// <summary>
        /// Get the collection of entities.
        /// </summary>
        /// <param name="firstResult">the first result to be retrieved. The value must be more than zero inclusive.</param>
        /// <param name="maxResults">the limit on the number of objects to be retrieved. The value must be more than zero.</param>
        /// <param name="username">Current user name.</param>
        /// <returns>found list of collection.</returns>
        IList<Order> List(int firstResult = 0, int maxResults = 50, string username = null);
    }
}
