using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShoppingCart.Models;
using ShoppingCart.Models.Domain;

namespace ShoppingCart
{
    public interface IProductService
    {
        /// <summary>
        /// Gets the collection of products.
        /// </summary>
        /// <returns>List of products</returns>
        IList<Product> List();
        /// <summary>
        /// Gets a product.
        /// </summary>
        /// <param name="id">entity id</param>
        /// <returns>found entity or null in case there's no entity with passed id found.</returns>
        Product Get(int id);
    }
}