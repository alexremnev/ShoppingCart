using System;
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
        public ActionResult List(string filter, string sortby, int? pageSize, int page = 0)
        {
            try
            {
                pageSize = pageSize > 50 ? pageSize : 50;
                var products = ProductService.List(filter, sortby, pageSize, page);
                return Json(products, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to get the list of products", e);
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
                if (product == null) return new HttpStatusCodeResult(500);
                return Json(product, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to get the product by id", e);
                return Json("Internal server error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Route]
        public HttpStatusCodeResult Create(Product entity)
        {
            try
            {
                ProductService.Create(entity);
                return new HttpStatusCodeResult(201);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried add a new product",e);
                return new HttpStatusCodeResult(400);
            }
            
        }
        [HttpDelete]
        [Route("{id:int}")]
        public HttpStatusCodeResult Delete(int id)
        {
            try
            {
                ProductService.Delete(id);
                return new HttpStatusCodeResult(204);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to delete the product by id", e);
                return new HttpStatusCodeResult(400);
            }
        }
    }
}