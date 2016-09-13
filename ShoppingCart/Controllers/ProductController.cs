using System.Collections.Generic;
using System.Web.Mvc;
using ShoppingCart.Models;
using ShoppingCart.Models.Domain;
using Spring.Context;
using Spring.Context.Support;

namespace ShoppingCart.Controllers
{
    [RoutePrefix("products")]
    public class ProductController : Controller
    {
        private static readonly IApplicationContext Ctx = ContextRegistry.GetContext();
        private readonly IProductRepository _repo = Ctx.GetObject<ProductRepository>();

        [Route]
        public ActionResult List()
        {
            IList<Product> jsonlist = _repo.Read();
            return Json(jsonlist, JsonRequestBehavior.AllowGet);
        }

        [Route("{id:int}")]
        public ActionResult GetById(int id)
        {
            var jsonsting = _repo.FindById(id);
            return Json(jsonsting, JsonRequestBehavior.AllowGet);
        }
    }
}