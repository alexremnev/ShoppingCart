using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Common.Logging;
using ShoppingCart.Business;
using ShoppingCart.DAL;


namespace ShoppingCart.Controllers
{
    [RoutePrefix("products")]
    public class ProductController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger<ProductController>();
        private const int StartPointPage = 1;
        private const int MaxPageSize = 50;
        private const bool DefaultSortDirection = true;
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Route]
        public ActionResult List(string filter, string sortby, bool? sortDirection, int? page, int? pageSize)
        {
            try
            {
                sortDirection = sortDirection ?? DefaultSortDirection;
                page = page ?? StartPointPage;
                page = page < 1 ? StartPointPage : page;
                pageSize = pageSize ?? MaxPageSize;
                pageSize = pageSize > 250 ? MaxPageSize : pageSize;
                pageSize = pageSize <= 0 ? MaxPageSize : pageSize;
                var firstResult = (page - 1) * pageSize;
                IList<Product> products = null;
                var count = _productService.Count(filter);
                if (count != 0) products = _productService.List(filter, sortby, sortDirection.Value, firstResult.Value, pageSize.Value);
                return Json(products, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to get the list of products", e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("count")]
        public ActionResult Count(string filter)
        {
            try
            {
                var count = _productService.Count(filter);
                return Json(count, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to get count of products", e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public ActionResult Get(int id)
        {
            try
            {
                if (id < 0) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                var product = _productService.Get(id);
                if (product == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                return Json(product, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error($"Exception occured when you tried to get the product by id={id}", e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route]
        public HttpStatusCodeResult Create(Product entity)
        {
            try
            {
                _productService.Create(entity);
                return new HttpStatusCodeResult(HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried add a new product", e);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }
        [HttpDelete]
        [Route("{id:int}")]
        public HttpStatusCodeResult Delete(int id)
        {
            try
            {
                if (id < 0) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                _productService.Delete(id);
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to delete the product by id", e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }
    }
}