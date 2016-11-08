using System.Collections.Generic;
using ShoppingCart.DAL;

namespace ShoppingCart.Business
{
    public interface IProductService :IService<Product>
    {
        /// <summary>
        /// Create a product.
        /// </summary>
        /// <param name="entity">entity product.</param>
        void Create(Product entity);
        /// <summary>
        /// Delete a product.
        /// </summary>
        /// <param name="id">entity id.</param>
        void Delete(int id);
        /// <summary>
        /// Get the collection of products.
        /// </summary>
        /// <param name="filter">filter of products or null.</param>
        /// <param name="sortby">the parameter of sorting or null.</param>
        /// <param name="sortDirection">the sort direction or null.</param>
        /// <param name="firstResult">the first result to be retrieved.</param>
        /// <param name="maxResults">the limit on the number of objects to be retrieved.</param>
        /// <returns>found list of products.</returns>
        IList<Product> List(string filter, string sortby, bool sortDirection, int firstResult, int maxResults);
        /// <summary>
        /// Get the count of products.
        /// </summary>
        /// <param name="filter">filter of products or null.</param>
        /// <returns>found number of products</returns>
        int Count(string filter);
        /// <summary>
        /// Update a product.
        /// </summary>
        /// <param name="entity">entity product.</param>
        void Update(Product entity);
        /// <summary>
        /// Get the product or null.
        /// </summary>
        /// <param name="name">entity name of product</param>
        /// <returns>found entity. If product not exist or amount of product more then 2 return null.</returns>
        Product GetByName(string name);
    }
}