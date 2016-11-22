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
        /// <param name="firstResult">the first result to be retrieved. The value must be more than zero inclusive.</param>
        /// <param name="maxResults">the limit on the number of objects to be retrieved. The value must be more than zero.</param>
        /// <param name="applyFilter">filter implementation.</param>
        /// <returns>found list of collection.</returns>
        IList<T> List(int firstResult = 0, int maxResults = 50, Func<IQueryOver<T, T>, IQueryOver<T, T>> applyFilter = null);
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
        /// <param name="applyFilter">filter implementation.</param>
        /// <returns>found number of entities.</returns> 
        int Count(Action<IQueryOver<T, T>> applyFilter = null);
    }
}
