using System.Web.Mvc;

namespace ShoppingCart.WebApi_3.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _service;
        public ProductController(IProductService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            return Content(_service.Get());
        }
    }
}
