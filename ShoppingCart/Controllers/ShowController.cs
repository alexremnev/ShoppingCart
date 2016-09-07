using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using ShoppingCart.Models.Domain;

namespace ShoppingCart.Controllers
{
    public class ShowController : Controller
    {
        // GET: Show
        public ActionResult Products()
        {
            List<Products> listProducts = new List<Products>();
            listProducts.Add(new Products()
            {
                Id = 1,
                Name = "Car",
                Quantity = 5,
                Price = 1000
            });

            listProducts.Add(new Products()
            {
                Id = 2,
                Name = "House",
                Quantity = 1,
                Price = 5000
            });

            listProducts.Add(new Products()
            {
                Id = 3,
                Name = "Ship",
                Quantity = 1,
                Price = 20000
            });
            List<Products> jsonList = new List<Products>();
            foreach (var product in listProducts)
            {
                jsonList.Add(product);
            }



            return Json(jsonList,JsonRequestBehavior.AllowGet);
        }
    }
}