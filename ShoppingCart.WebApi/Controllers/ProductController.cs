using System;
using System.Net;
using System.Web.Http;
using Common.Logging;
using ShoppingCart.Business;
using ShoppingCart.DAL;
using ShoppingCart.DAL.NHibernate;

namespace ShoppingCart.WebApi.Controllers
{
    public class ProductController : BaseController<Product>
    {
        public ITestService Service { get; set; }

        private const string Controller = "product";
        private static readonly ILog Log = LogManager.GetLogger<ProductController>();
        private const int FirstPage = 1;
        private const int MaxPageSize = 50;
        private const bool DefaultSortDirection = true;
        private readonly IProductService _productService;

        public ProductController(IProductService productService, ISecurityContext context) : base(Controller, context)
        {
            _productService = productService;
        }

        // GET api/product/list
        [AllowAnonymous]
        [HttpGet]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller)]
        public IHttpActionResult List(string filter = null, string sortby = null,
            bool? sortDirection = DefaultSortDirection, int? page = FirstPage, int? pageSize = MaxPageSize)
        {
            return base.List(filter, sortby, sortDirection, page, pageSize);
        }

        // GET api/product/count
        [HttpGet]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller + "/count")]
        public new IHttpActionResult Count(string filter = null, decimal maxPrice = 0)
        {
            return base.Count(filter, maxPrice);
        }

        // GET api/product/id
        [HttpGet]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller + "/{id}")]
        public IHttpActionResult GetById(int id)
        {
            return Get(id);
        }

        // POST api/product
        [Authorize(Roles = "admin, superUser")]
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
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller + "/{id}")]
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

        protected override IService<Product> GetService()
        {
            return _productService;
        }
    }
}
