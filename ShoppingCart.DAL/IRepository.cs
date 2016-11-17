using System;
using System.Collections.Generic;
using NHibernate;

namespace ShoppingCart.DAL
{
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Create an entity.
        /// </summary>
        /// <param name="entity">list of entity. If entity is null occurs RepositoryException.</param>
        void Create(T entity);
        /// <summary>
        /// Get the collection of entities.
        /// </summary>
        /// <param name="isAscending">the sort direction or null. If isAscending is null or true, the list of entities sorts by asc. Possible options are true or false. Valid only for etity product.</param>
        /// <param name="firstResult">the first result to be retrieved. The value must be more than zero inclusive.</param>
        /// <param name="maxResults">the limit on the number of objects to be retrieved. The value must be more than zero.</param>
        /// <param name="filter">filter of entities or null. If filter is null, returns all elements. Valid only for etity product.</param>
        /// <param name="sortby">the parameter of sorting or null. If filter is null, list of entities sorts by id.Possible options are id, price, quantity. Valid only for etity product.</param>
        /// <param name="applyFilters">filter implementation.</param>
        /// <returns>found list of collection.</returns>
        IList<T> List(string filter = null, string sortby = null, bool isAscending = true,
             int firstResult = 0, int maxResults = 50, Func<string, string, bool, IQueryOver<T, T>, IQueryOver<T, T>> applyFilters = null);
        /// <summary>
        /// Get an entity.
        /// </summary>
        /// <param name="id">entity id.</param>
        /// <returns>found entity or null in case there's no entity with passed id found.</returns>
        T Get(int id);
        /// <summary>
        /// Update an entity.
        /// </summary>
        /// <param name="entity">if entity is null occurs RepositoryException.</param>
        void Update(T entity);
        /// <summary>
        /// Create a new entity.
        /// </summary>
        /// <returns>new entity.</returns>
        T NewEntity();

        /// <summary>
        /// Get the count of entities.
        /// </summary>
        /// <param name="filter">filter of entity or null. If filter is null, returns all elements.</param>
        /// <param name="maxPrice">max price of entity.</param>
        /// <param name="maxPriceFilter">filter by price.</param>
        /// <param name="applyFilter">filter implementation.</param>
        /// <returns>found number of entities.</returns> 
        int Count(string filter = null, decimal maxPrice = 0, Action<decimal, IQueryOver<T, T>> maxPriceFilter = null, Action<string, IQueryOver<T, T>> applyFilter = null);
    }
}
