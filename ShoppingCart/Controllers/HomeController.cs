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
            var firstProduct = new Products {Id=0, Name = "Car", Quantity = 5, Price = 1000.0 };
            repo.Add(firstProduct);


            return View();
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