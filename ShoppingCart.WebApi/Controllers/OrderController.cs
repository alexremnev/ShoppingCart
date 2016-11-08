using System;
using System.Linq;
using System.Web.Http;
using Common.Logging;
using ShoppingCart.Business;
using ShoppingCart.DAL;

namespace ShoppingCart.WebApi.Controllers
{
    public class OrderController : BaseController<Order>
    {
        private const string Controller = "order";
        private static readonly ILog Log = LogManager.GetLogger<OrderController>();
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public OrderController(IOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }

        //api/order
        [HttpGet]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller)]
        public new IHttpActionResult List(int? page = null, int? pageSize = null)
        {
            try
            {
                return base.List(page, pageSize);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to get the list of orders", e);
                return InternalServerError();
            }
        }

        // api/order/place
        [HttpPost]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller)]
        public IHttpActionResult Place([FromBody] Order entity)
        {
            try
            {
                if (entity == null) return BadRequest("Entity is not valid");
                var products = entity.LineItems;
                if (products == null) return BadRequest("lineItems are empty");
                foreach (var lineItem in products)
                {
                    var id = lineItem.ProductId;
                    var product = _productService.Get(id);
                    if (product != null) continue;
                    return BadRequest($"Product with id = {id} is not exist");
                }

                if (products.Any(product => product.Quantity < 0))
                {
                    return BadRequest("Quantity can't be less then 0");
                }
                _orderService.Place(entity);
                return CreatedAtRoute(WebApiConfig.DefaultRoute, new { controller = Controller, id = entity.Id }, entity.Id);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to place an order", e);
                return InternalServerError();
            }
        }
        // GET api/order/id
        [HttpGet]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller+"/{id}")]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                return Get(id);
            }
            catch (Exception e)
            {
                Log.Error($"Exception occured when you tried to get the customer by id={id}", e);
                return InternalServerError();
            }
        }

        public override IService<Order> GetService()
        {
            return _orderService;
        }
    }
}
