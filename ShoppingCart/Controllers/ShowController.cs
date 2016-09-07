using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using ShoppingCart.Models;
using ShoppingCart.Models.Domain;

namespace ShoppingCart.Controllers
{
    public class ShowController : Controller
    {
        // GET: Show
        public ActionResult Products()
        {
            
            ProductsRepository repo = new ProductsRepository();
            List<Products> jsonlist = repo.ShowAllProducts();
            
                


            return Json(jsonlist,JsonRequestBehavior.AllowGet);
        }
    }
}