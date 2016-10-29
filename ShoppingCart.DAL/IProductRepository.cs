using System.Collections.Generic;

namespace ShoppingCart.DAL
{
    public interface IProductRepository
    {
        /// <summary>
        /// Create a product.
        /// </summary>
        /// <param name="entity">entity product. If product is null occurs RepositoryException.</param>
        void Create(Product entity);
        /// <summary>
        /// Get a product.
        /// </summary>
        /// <param name="id">entity id.</param>
        /// <returns>found entity or null in case there's no entity with passed id found.</returns>
        Product Get(int id);
        /// <summary>
        /// Delete a product.
        /// </summary>
        /// <param name="id">entity id. If id is not exist occurs RepositoryException.</param>
        void Delete(int id);
        /// <summary>
        /// Get the collection of products.
        /// </summary>
        /// <param name="filter">filter of products or null. If filter is null, returns all elements.</param>
        /// <param name="sortby">the parameter of sorting or null. If filter is null, list of products sorts by id.Possible options are id, price, quantity.</param>
        /// <param name="isAscending">the sort direction or null. If isAscending is null or true, the list of products sorts by asc. Possible options are true or false.</param>
        /// <param name="firstResult">the first result to be retrieved. The value must be more than zero inclusive.</param>
        /// <param name="maxResults">the limit on the number of objects to be retrieved. The value must be more than zero.</param>
        /// <returns>found list of products.</returns>
        IList<Product> List(string filter = null, string sortby = null, bool isAscending = true, int firstResult = 0, int maxResults = 50);
        /// <summary>
        /// Get the count of products.
        /// </summary>
        /// <param name="filter">filter of products or null. If filter is null, returns all elements.</param>
        /// <returns>found number of products.</returns>
        int Count(string filter = null);
        /// <summary>
        /// Update a product.
        /// </summary>
        /// <param name="entity">entity product. If product is null occurs RepositoryException.</param>
        void Update(Product entity);
        /// <summary>
        /// Get the collection of products.
        /// </summary>
        /// <param name="name">entity name of product</param>
        /// <returns>found entity or null.</returns>
        IList<Product> GetByName(string name);
    }
}
