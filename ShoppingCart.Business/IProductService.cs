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