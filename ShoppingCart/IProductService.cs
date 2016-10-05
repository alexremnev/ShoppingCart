using System.Collections.Generic;
using ShoppingCart.DAL;

namespace ShoppingCart
{
    public interface IProductService
    {
        /// <summary>
        /// Create a product.
        /// </summary>
        /// <param name="entity">entity product</param>
        void Create(Product entity);
        /// <summary>
        /// Gets a product.
        /// </summary>
        /// <param name="id">entity id</param>
        /// <returns>found entity or null in case there's no entity with passed id found.</returns>
        Product Get(int id);
        /// <summary>
        /// Delete a product.
        /// </summary>
        /// <param name="id">entity id</param>
        void Delete(int id);
        /// <summary>
        /// Get the collection and number of products.
        /// </summary>
        /// <param name="filter">filter of products or null. If filter is null, returns all elements.</param>
        /// <param name="sortby">the parameter of sorting or null. If filter is null, list of products sorts by id.Possible options are id, price, quantity.</param>
        /// <param name="sortDirection">the sort direction or null. If sortDirection is null, list of products sorts by asc. Possible options are desc or asc.</param>
        /// <param name="firstResult">the first result to be retrieved. The value must be more than zero.</param>
        /// <param name="maxResults">the limit on the number of objects to be retrieved. The value must be more than zero.</param>
        /// <returns>found list of products and their number.</returns>
        IList<Product> List(string filter, string sortby, string sortDirection, int firstResult, int maxResults);
        /// <summary>
        /// Get the count of products
        /// </summary>
        /// <param name="filter">filter of products or null. If filter is null, returns all elements.</param>
        /// <returns>found number of products</returns>
        int Count(string filter);
    }
}