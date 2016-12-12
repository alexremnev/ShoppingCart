using System.Web.Http;
using System.Web.Mvc;
using ShoppingCart.WebApi_2.Service;

namespace ShoppingCart.WebApi_2.Controllers
{
   public class CustomerController : Controller
    {
        private readonly IProductService _service;
        public CustomerController(IProductService service)
        {
            _service = service;
        }
   
        public ActionResult Show()
        {
            var value = _service.Get();
            return Content(value);
        }

      
    }
}
