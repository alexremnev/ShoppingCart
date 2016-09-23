using System.Collections.Generic;
using ShoppingCart.Models.Domain;

namespace ShoppingCart.Models
{
   public interface IProductRepository
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
        /// Gets the collection of products.
        /// </summary>
        /// <param name="filter">filter of products</param>
        /// <param name="sortby">parameter of sorting</param>
        /// <param name="maxResult">quantity of element on one page</param>
        /// <param name="firstResult">number of page</param>
        /// <returns>List of products</returns>
        IList<Product> List(string filter, string sortby, int? maxResult, int? firstResult);
    }
}