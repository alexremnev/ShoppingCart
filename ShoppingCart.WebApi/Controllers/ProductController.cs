using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Common.Logging;
using ShoppingCart.DAL;
using ShoppingCart.ProductService;

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

        // GET api/product/{filter}/{srotby}/{sortDirection}/{page}/{pageSize}
        [HttpGet]
        public IHttpActionResult Get(string filter, string sortby, bool? sortDirection, int? page, int? pageSize)
        {
            Log.Fatal("Hello Log");
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
                products = products ?? new List<Product>();
                return Ok(products);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to get the list of products", e);
                return InternalServerError();
            }
        }

        // GET api/product/count/{filer}
        [HttpGet]
        public IHttpActionResult Count(string filter)
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
        public IHttpActionResult Get(int id)
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
        public IHttpActionResult Post([FromBody]Product entity)
        {
            try
            {
                if (entity == null) return BadRequest();
                // if (entity.Name.Length > 50) BadRequest("Name consists of more than 50 letters");//todo second variant;
                // if (entity.Name.Length > 50) throw ValidationException("Name consists of more than 50 letters");//todo third variant;
                _productService.Create(entity);
                var id = entity.Id;
                var uri = new Uri($"http://localhost:50896/api/product/{id}");
                return Created(uri, id);
            }
            catch (ValidationException e)
            {
                Log.Error("Exception occured when you tried to add a new product", e);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to add a new product", e);
                return InternalServerError();
            }
        }

        //PUT api/product/id
        public IHttpActionResult Put(int id, [FromBody] Product product)
        {
            if ((id < 0) || (product == null))
            {
                Log.Error("Entity or id is not valid");
                return BadRequest();
            }
            try
            {
                if (_productService.Update(id, product))
                    return StatusCode(HttpStatusCode.NoContent);
                return BadRequest();
            }
            catch (ValidationException e)
            {
                Log.Error("Exception occured when you tried to update a product", e);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to update a product", e);
                return InternalServerError();
            }
        }

        // DELETE api/product/id
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
