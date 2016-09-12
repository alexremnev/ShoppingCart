using System.Collections.Generic;
using System.Web.Mvc;
using ShoppingCart.Models;
using ShoppingCart.Models.Domain;

namespace ShoppingCart.Controllers
{

    public class ProductController : Controller
    {
        private readonly IProductRepository _repo = new ProductRepository();
        //public ProductController(IProductRepository repo)
        //{
        //    _repo = repo;
        //}
        //public ProductController() { }




        [Route("Products")]
        public ActionResult Products()
        {
            
            List<Product> jsonlist = _repo.ShowAllProducts();
            return Json(jsonlist,JsonRequestBehavior.AllowGet);
        }



        [Route("Products/{id:int}")]
        public ActionResult Products(int id)
        {
            var jsonsting = _repo.FindById(id);
            return Json(jsonsting, JsonRequestBehavior.AllowGet);
        }
        
    }
}