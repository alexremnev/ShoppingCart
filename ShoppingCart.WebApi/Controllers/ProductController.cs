using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Common.Logging;
using ShoppingCart.DAL;
using ShoppingCart.ProductService;

namespace ShoppingCart.WebApi.Controllers
{
    public class ProductController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger<ProductController>();
        private const int DefaultPageResult = 1;
        private const int DefaultMaxResult = 50;
        private const bool DefaultSortDirection = true;
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET api/product
        [HttpGet]
        public HttpResponseMessage Get(string filter, string sortby, bool? sortDirection, int? page, int? pageSize)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            Log.Fatal("Hello Log");
            try
            {
                sortDirection = sortDirection ?? DefaultSortDirection;
                page = page ?? DefaultPageResult;
                page = page < 1 ? DefaultPageResult : page;
                pageSize = pageSize ?? DefaultMaxResult;
                pageSize = pageSize > 250 ? DefaultMaxResult : pageSize;
                pageSize = pageSize <= 0 ? DefaultMaxResult : pageSize;
                var firstResult = (page - 1) * pageSize;
                IList<Product> products = null;
                var count = _productService.Count(filter);
                if (count != 0)
                    products = _productService.List(filter, sortby, sortDirection.Value, firstResult.Value, pageSize.Value);
                response.Content = new ObjectContent(typeof(List<Product>), products, new JsonMediaTypeFormatter());
               }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to get the list of products", e);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
           return response;
        }

        // GET api/product/count
        [HttpGet]
        public HttpResponseMessage Count(string filter)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                var count = _productService.Count(filter);
                response.Content = new ObjectContent(typeof(int), count, new JsonMediaTypeFormatter());
            }
            catch (Exception e)
            {
                Log.Error($"Exception occured when you tried to get count of products by filter={filter}", e);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        // GET api/product/5
        public HttpResponseMessage Get(int id)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                if (id < 0) return new HttpResponseMessage(HttpStatusCode.NotFound);
                var product = _productService.Get(id);
                if (product == null) return new HttpResponseMessage(HttpStatusCode.NotFound);
                response.Content = new ObjectContent(typeof(Product), product, new JsonMediaTypeFormatter());
            }
            catch (Exception e)
            {
                Log.Error($"Exception occured when you tried to get the product by id={id}", e);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        // POST api/product
        public HttpResponseMessage Post([FromBody]Product entity)
        {
            var response = new HttpResponseMessage(HttpStatusCode.Created);
            try
            {
                _productService.Create(entity);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried add a new product", e);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return response;
        }

        // DELETE api/product/5
        public HttpResponseMessage Delete(int id)
        {
            var response = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                if (id < 0) return new HttpResponseMessage(HttpStatusCode.BadRequest);
                _productService.Delete(id);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to delete the product by id", e);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            return response;
        }
    }
}
