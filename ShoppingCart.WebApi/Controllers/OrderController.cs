using System;
using System.Linq;
using System.Net;
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

        public OrderController(IOrderService orderService, IProductService productService) : base(Controller)
        {
            _orderService = orderService;
            _productService = productService;
        }

        //api/order
        [HttpGet]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller)]
        public IHttpActionResult List(int? page = null, int? pageSize = null)
        {
            return List(null, null, true, page, pageSize);
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
                    return BadRequest("Quantity can't be less than 0");
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
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller + "/{id}")]
        public IHttpActionResult GetById(int id)
        {
            return Get(id);
        }

        protected override IService<Order> GetService()
        {
            return _orderService;
        }

        //PUT api/order
        [HttpPut]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller)]
        public IHttpActionResult Update([FromBody] Order order)
        {
            try
            {
                if (order == null) return BadRequest("Entity is not valid");
                var orderId = order.Id;
                if (orderId < 0) return BadRequest("Order id is not valid");
                var existOrder = _orderService.Get(orderId);
                if (existOrder == null) return BadRequest("Order is not exist");
                var products = order.LineItems;
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
                    return BadRequest("Quantity can't be less than 0");
                }
                existOrder.SaleDate = order.SaleDate;
                _orderService.ReturnProducts(existOrder.LineItems);
                for (var i = 0; i < existOrder.LineItems.Count; i++)
                {
                    existOrder.LineItems[i].ProductId = order.LineItems[i].ProductId;
                    existOrder.LineItems[i].Quantity = order.LineItems[i].Quantity;
                }
                _orderService.Update(existOrder);
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when you tried to update an order", e);
                return InternalServerError();
            }
        }

        // GET api/order/count/
        [HttpGet]
        [Route(WebApiConfig.SegmentOfRouteTemplate + Controller + "/count")]
        public IHttpActionResult Count()
        {
            return base.Count();
        }
    }
}
