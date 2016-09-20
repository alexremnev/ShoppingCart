using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Common.Logging;
using ShoppingCart.Models.Domain;

namespace ShoppingCart.Controllers
{
    [RoutePrefix("products")]
    public class ProductController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger<ProductController>();

        public IProductService ProductService { get; set; }

        [HttpGet]
        [Route]
        public ActionResult List(string filter, string sortBy, int page = 0, int pageSize = 20)
        {
            try
            {
                IEnumerable<Product> products = ProductService.List();
                if (filter != null) products = products.Where(x => x.Name == filter);
                if (sortBy != null)
                    switch (sortBy)
                    {
                        case "Price":
                            products = products.OrderBy(x => x.Price);
                            break;
                        case "Quantity":
                            products = products.OrderBy(x => x.Quantity);
                            break;
                        default:
                            products = products.OrderBy(x => x.Name);
                            break;
                    }
                products = products.Skip(page * pageSize).Take(pageSize);
                return Json(products, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you try to get the list of products", e);
                return Json("Internal server error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public ActionResult Get(int id)
        {
            try
            {
                var product = ProductService.Get(id);
                return Json(product, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you try to get the product by id", e);
                return Json("Internal server error", JsonRequestBehavior.AllowGet);
            }
        }
    }
}