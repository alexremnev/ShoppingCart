using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        public void Can_get_product_by_id()
        {
            //Arrange
            const int id = 7;
            var mock = new Mock<IProductService>();
            var expected = new Product { Id = id, Name = "Apple", Quantity = 30, Price = 20 };
            mock.Setup(m => m.Get(id)).Returns(expected);
            var controller = new ProductController(mock.Object);

            //Act
            var jsonResult = controller.Get(id) as JsonResult;
            if (jsonResult == null) return;
            var actual = (Product)jsonResult.Data;

            //Assert
            Assert.AreEqual(expected, actual);
            CompareProducts(expected, actual);
        }

        private static void CompareProducts(Product expected, Product actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Price, actual.Price);
            Assert.AreEqual(expected.Quantity, actual.Quantity);
        }



    }
}
