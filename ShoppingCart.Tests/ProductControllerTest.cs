using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using ShoppingCart.Controllers;
using ShoppingCart.DAL;

namespace ShoppingCart.Tests
{
    [TestClass]
    public class ProductControllerTest
    {
        private readonly IList<Product> _list = new List<Product>
        {
            new Product {Id = 1, Name = "Apple", Quantity = 30, Price = 20},
            new Product {Id = 2, Name = "Apple", Quantity = 5, Price = 40},
            new Product {Id = 3, Name = "Apple", Quantity = 25, Price = 30}
        };
       
        [TestMethod]
        public void GetMethod()
        {
            //Arrange
            const int id = 1;
            var mock = new Mock<IProductService>();
            var product = new Product { Id = 1, Name = "Apple", Quantity = 30, Price = 20 };
            mock.Setup(m => m.Get(id)).Returns(product);
            var controller = new ProductController(mock.Object);

            //Act
            var expected = JsonConvert.SerializeObject(product);
            var actualProduct = controller.Get(id) as JsonResult;

            //Assert
            Assert.AreEqual(expected, actualProduct);
        }


    }
}
