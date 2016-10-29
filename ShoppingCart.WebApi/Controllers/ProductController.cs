using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using Common.Logging;
using ShoppingCart.Business;
using ShoppingCart.DAL;

namespace ShoppingCart.WebApi.Controllers
{
    public class ProductController : ApiController
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

        // GET api/product/list
        [HttpGet]
        [Route("api/product")]
        public IHttpActionResult List(string filter = null, string sortby = null, bool? sortDirection = DefaultSortDirection, int? page = StartPointPage, int? pageSize = MaxPageSize)
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
                var products = _productService.List(filter, sortby, sortDirection.Value, firstResult.Value, pageSize.Value);
                return Ok(products);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to get the list of products", e);
                return InternalServerError();
            }
        }

        // GET api/product/count/
        [HttpGet]
        public IHttpActionResult Count(string filter = null)
        {
            try
            {
                var count = _productService.Count(filter);
                return Ok(count);
            }
            catch (Exception e)
            {
                Log.Error($"Exception occured when you tried to get count of products by filter={filter}", e);
                return InternalServerError();
            }
        }

        // GET api/product/id
        [HttpGet]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                if (id < 0) return BadRequest();
                var product = _productService.Get(id);
                return product == null ? (IHttpActionResult)NotFound() : Ok(product);
            }
            catch (Exception e)
            {
                Log.Error($"Exception occured when you tried to get the product by id={id}", e);
                return InternalServerError();
            }
        }

        // POST api/product
        [HttpPost]
        [Route("api/product")]
        public IHttpActionResult Create([FromBody]Product entity)
        {

            try
            {
                if (entity == null) return BadRequest("Entity is not valid");
                if (string.IsNullOrEmpty(entity.Name)) return BadRequest("Name is empty");
                if (entity.Name.Length > 50) return BadRequest("Name consists of more than 50 letters");
                var products = _productService.GetByName(entity.Name);
                if (products.Count != 0) return BadRequest("Name is not unique");
                _productService.Create(entity);
                return CreatedAtRoute("DefaultApi", new { controller = "product", id = entity.Id }, entity.Id);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to add a new product", e);
                return InternalServerError();
            }
        }

        //PUT api/product
        [HttpPut]
        [Route("api/product")]
        public IHttpActionResult Update([FromBody] Product product)
        {
            try
            {
                if (product == null) return BadRequest("Entity is not valid");
                if (product.Id < 0) return BadRequest("Id is not valid");
                if (string.IsNullOrEmpty(product.Name)) return BadRequest("Name is empty");
                if (product.Name.Length > 50) return BadRequest("Name consists of more than 50 letters");
                var products = _productService.GetByName(product.Name);
                if (!((products.Count == 0) | ((products.Count == 1) && products.First().Id == product.Id)))
                    return BadRequest("Name is not unique");
                _productService.Update(product);
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to update a product", e);
                return InternalServerError();
            }
        }

        // DELETE api/product/id
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                if (id < 0) return BadRequest();
                _productService.Delete(id);
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to delete the product by id", e);
                return InternalServerError();
            }
        }
    }
}
