using System.Collections.Generic;

namespace ShoppingCart.Business
{
    public interface IService<TEntity>
    {
        /// <summary>
        /// Get the collection of entity.
        /// </summary>
        /// <param name="firstResult">the first result to be retrieved.</param>
        /// <param name="maxResults">the limit on the number of objects to be retrieved.</param>
        /// <returns>found list of entities.</returns>
        IList<TEntity> List(int firstResult, int maxResults);
        /// <summary>
        /// Get an entity.
        /// </summary>
        /// <param name="id">entity id.</param>
        /// <returns>found entity or null.</returns>
        TEntity Get(int id);
    }
}
