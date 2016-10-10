using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Common.Logging;
using ShoppingCart.DAL;


namespace ShoppingCart.Controllers
{
    [RoutePrefix("products")]
    public class ProductController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger<ProductController>();
        private const int DefaultPageResult = 0;
        private const int DefaultMaxResult = 50;
        private const bool DefaultSortDirection = true;
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Route]
        public ActionResult List(string filter, string sortby, bool? sortDirection, int? pageResult, int? maxResults)
        {
            try
            {
                sortDirection = sortDirection ?? DefaultSortDirection;
               pageResult = pageResult ?? DefaultPageResult;
                pageResult = pageResult < 0 ? DefaultPageResult : pageResult;
                maxResults = maxResults ?? DefaultMaxResult;
                maxResults = maxResults > 250 ? DefaultMaxResult : maxResults;
                maxResults = maxResults <= 0 ? DefaultMaxResult : maxResults;
                var firstResult = pageResult * maxResults;
                IList<Product> products = null;
                var count = _productService.Count(filter);
                if (count != 0) products = _productService.List(filter, sortby, sortDirection.Value, firstResult.Value, maxResults.Value);
                var productsAndCount = new { products, count };
                return Json(productsAndCount, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to get the list of products", e);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("count")]
        public ActionResult Count(string filter)
        {
            try
            {
                var count = new { count = _productService.Count(filter) };
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to get count of products", e);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public ActionResult Get(int id)
        {
            try
            {
                if (id < 0) return new HttpStatusCodeResult(400);
                var product = _productService.Get(id);
                if (product == null) return new HttpStatusCodeResult(500);
                return Json(product, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to get the product by id", e);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpPost]
        [Route]
        public HttpStatusCodeResult Create(Product entity)
        {
            try
            {
                _productService.Create(entity);
                return new HttpStatusCodeResult(201);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried add a new product", e);
                return new HttpStatusCodeResult(400);
            }

        }
        [HttpDelete]
        [Route("{id:int}")]
        public HttpStatusCodeResult Delete(int id)
        {
            try
            {
                _productService.Delete(id);
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