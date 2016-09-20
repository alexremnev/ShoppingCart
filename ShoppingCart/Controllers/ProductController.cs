using System;
using System.Web.Mvc;
using Common.Logging;

namespace ShoppingCart.Controllers
{
    [RoutePrefix("products")]
    public class ProductController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger<ProductController>();

        public IProductService ProductService { get; set; }

        [HttpGet]
        [Route]
        public ActionResult List()
        {
            try
            {
                var products = ProductService.List();
                return Json(products, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return Json("Internal server error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public ActionResult Get(int id)
        {
            try
            {
                var product = ProductService.Get(id);
                return Json(product, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return Json("Internal server error", JsonRequestBehavior.AllowGet);
            }
        }
    }
}