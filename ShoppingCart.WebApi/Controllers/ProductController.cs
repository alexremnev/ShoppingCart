using System;
using System.Net;
using System.Web.Http;
using Common.Logging;
using ShoppingCart.Business;
using ShoppingCart.DAL;

namespace ShoppingCart.WebApi.Controllers
{
    public class ProductController : BaseController<Product>
    {
        private const string Controller = "product";
        private static readonly ILog Log = LogManager.GetLogger<ProductController>();
        private const int FirstPage = 1;
        private const int MaxPageSize = 50;
        private const bool DefaultSortDirection = true;
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET api/product/list
        [HttpGet]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller)]
        public IHttpActionResult List(string filter = null, string sortby = null, bool? sortDirection = DefaultSortDirection, int? page = FirstPage, int? pageSize = MaxPageSize)
        {
            try
            {
                sortDirection = sortDirection ?? DefaultSortDirection;
                page = page ?? FirstPage;
                page = page < 1 ? FirstPage : page;
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
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller + "/count")]
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
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller + "/{id}")]
        public  IHttpActionResult GetById(int id)
        {
            try
            {
                return Get(id);
            }
            catch (Exception e)
            {
                Log.Error($"Exception occured when you tried to get the product by id={id}", e);
                return InternalServerError();
            }
        }

        // POST api/product
        [HttpPost]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller)]
        public IHttpActionResult Create([FromBody]Product entity)
        {
            try
            {
                if (entity == null) return BadRequest("Entity is not valid");
                if (string.IsNullOrEmpty(entity.Name)) return BadRequest("Name is empty");
                if (entity.Name.Length > 50) return BadRequest("Name consists of more than 50 letters");
                if (entity.Price < 0) return BadRequest("Price can't be less then 0");
                if (entity.Quantity < 0) return BadRequest("Quantity can't be less then 0");
                var products = _productService.GetByName(entity.Name);
                if (products != null) return BadRequest("Name is not unique");
                _productService.Create(entity);
                return CreatedAtRoute(WebApiConfig.DefaultRoute, new { controller = Controller, id = entity.Id }, entity.Id);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to add a new product", e);
                return InternalServerError();
            }
        }

        //PUT api/product
        [HttpPut]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller)]
        public IHttpActionResult Update([FromBody] Product product)
        {
            try
            {
                if (product == null) return BadRequest("Entity is not valid");
                var productId = product.Id;
                if (productId < 0) return BadRequest("Id is not valid");
                if (string.IsNullOrEmpty(product.Name)) return BadRequest("Name is empty");
                if (product.Name.Length > 50) return BadRequest("Name consists of more than 50 letters");
                if (product.Price < 0) return BadRequest("Price can't be less then 0");
                if (product.Quantity < 0) return BadRequest("Quantity can't be less then 0");
                var productByName = _productService.GetByName(product.Name);
                if ((productByName != null) && (productByName.Id != productId)) return BadRequest("Name is not unique");
                Product oldProduct;
                if ((productByName != null) && (productByName.Id == productId))
                    oldProduct = productByName;
                else
                {
                    oldProduct = _productService.Get(productId);
                    if (oldProduct == null) return BadRequest($"Product with id={productId} not exist");
                }
                oldProduct.Name = product.Name;
                oldProduct.Quantity = product.Quantity;
                oldProduct.Price = product.Price;
                _productService.Update(oldProduct);
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

        public override IService<Product> GetService()
        {
            return _productService;
        }
    }
}
