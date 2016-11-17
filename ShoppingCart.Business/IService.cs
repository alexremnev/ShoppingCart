using System.Collections.Generic;

namespace ShoppingCart.Business
{
    public interface IService<TEntity>
    {
        /// <summary>
        /// Get the collection of entity.
        /// </summary>
        /// <param name="filter">filter of entities or null. If filter is null, returns all elements. Valid only for etity product.</param>
        /// <param name="sortby">the parameter of sorting or null. If filter is null, list of entities sorts by id.Possible options are id, price, quantity. Valid only for etity product.</param>
        /// <param name="isAscending">the sort direction or null. If isAscending is null or true, the list of entities sorts by asc. Possible options are true or false. Valid only for etity product.</param>
        /// <param name="firstResult">the first result to be retrieved.</param>
        /// <param name="maxResults">the limit on the number of objects to be retrieved.</param>
        /// <returns>found list of entities.</returns>
        IList<TEntity> List(string filter = null, string sortby = null, bool isAscending = true, int firstResult = 0, int maxResults = 50);
        /// <summary>
        /// Get an entity.
        /// </summary>
        /// <param name="id">entity id.</param>
        /// <returns>found entity or null.</returns>
        TEntity Get(int id);
        /// <summary>
        /// Get the count of entities.
        /// </summary>
        /// <param name="filter">filter of entity or null.</param>
        /// <returns>found number of entities</returns>
        int Count(string filter,decimal maxPrice);

    }
}
