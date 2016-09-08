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
            
            return View();
        }
        
    }
    

}