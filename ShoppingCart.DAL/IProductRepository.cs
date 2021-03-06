﻿using System.Collections.Generic;

namespace ShoppingCart.DAL
{
    public interface IProductRepository : IRepository<Product>
    {
        /// <summary>
        /// Delete a product.
        /// </summary>
        /// <param name="id">entity id.</param>
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
        /// Get the product or null.
        /// </summary>
        /// <param name="name">entity name of product</param>
        /// <returns>found entity. If product not exist or amount of product more then 2 return null.</returns>
        Product GetByName(string name);
        /// <summary>
        /// Get the count of entities.
        /// </summary>
        /// <param name="filter">filter of entity or null. If filter is null, returns all elements.</param>
        /// <param name="maxPrice">max price of entity.</param>
        /// <returns>found number of entities.</returns> 
        int Count(string filter = null, decimal maxPrice = 0);
    }
}
