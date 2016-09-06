using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using ShoppingCart.Models;
using ShoppingCart.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace ShoppingCart.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {

            ProductsRepository repo = new ProductsRepository();
            var firstProduct = new Products { Id = new Guid(), Name = "Car", Quantity = 5, Price = 1000 };
            repo.Add(firstProduct);
            List<Products> listProducts = new List<Products>();
            listProducts.Add(new Products()
            {
                Id = new Guid(),
                Name = "Car",
                Quantity = 5,
                Price = 1000
            });
            var jsonstring = JsonConvert.SerializeObject(listProducts);


            return View(jsonstring);
        }
        public static void LoadNHibernateCfg()
        {
            var cfg = new Configuration();
            cfg.Configure();
            cfg.AddAssembly(typeof(Products).Assembly);
            new SchemaExport(cfg).Execute(true, true, false);
        }
    }
    

}