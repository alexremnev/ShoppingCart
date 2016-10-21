using System.Collections.Generic;
using System.Net;
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
        [TestMethod]
        public void Can_get_list()
        {
            //Arrange
            IList<Product> list = new List<Product>
        {
            new Product {Id = 1, Name = "Car yellow", Quantity = 5, Price = 15000},
            new Product {Id = 2, Name = "car blue", Quantity = 7, Price = 20000},
            new Product {Id = 3, Name = "apple oneType", Quantity = 3, Price = 40},
            new Product {Id = 5, Name = "Apple anotherType", Quantity = 5, Price = 37},
            new Product {Id = 5, Name = "apple", Quantity = 25, Price = 40}
        };
            var expected = list;
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.Count(null)).Returns(list.Count);
            mock.Setup(m => m.List(null, null, true, 0, 5)).Returns(list);
            var controller = new ProductController(mock.Object);

            //Act
            var jsonResult = controller.List(null, null, true, 1, 5) as JsonResult;
            Assert.IsNotNull(jsonResult);
            var actual = jsonResult.Data as IList<Product>;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Can_get_list_with_incorect_page()
        {
            //Arrange
            const int count = 5;
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.Count(It.IsAny<string>())).Returns(count);
            var controller = new ProductController(mock.Object);
            var listIncorrectPageList = new List<int> { -100, -1, 0 };
            //Act
            foreach (var incorrectPage in listIncorrectPageList)
            {
                controller.List(null, null, null, incorrectPage, 5);
                //Assert
                mock.Verify(ps => ps.List(null, null, true, 0, 5));
            }
        }

        [TestMethod]
        public void Can_get_list_with_incorect_pageSize()
        {
            //Arrange
            const int count = 5;
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.Count(It.IsAny<string>())).Returns(count);
            var controller = new ProductController(mock.Object);
            var incorrectPageSizeList = new List<int> { -1, 0, 251 };
            //Act
            foreach (var incorrectPageSize in incorrectPageSizeList)
            {
                controller.List(null, null, null, 1, incorrectPageSize);
                //Assert
                mock.Verify(ps => ps.List(null, null, true, 0, 50));
            }
        }

        [TestMethod]
        public void Can_get_count()
        {
            //Arrange
            var mock = new Mock<IProductService>();
            const int count = 7;
            var expected = count;
            mock.Setup(m => m.Count(It.IsAny<string>())).Returns(count);
            var controller = new ProductController(mock.Object);

            //Act
            var jsonResult = controller.Count(null) as JsonResult;

            //Assert
            Assert.IsNotNull(jsonResult);
            var actual = jsonResult.Data;
            Assert.IsInstanceOfType(actual, typeof(int));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Can_get_product_by_id()
        {
            //Arrange
            const int id = 7;
            var mock = new Mock<IProductService>();
            var expected = new Product { Id = id, Name = "Car yellow", Quantity = 5, Price = 15000 };
            mock.Setup(m => m.Get(id)).Returns(expected);
            var controller = new ProductController(mock.Object);

            //Act
            var jsonResult = controller.Get(id) as JsonResult;
            Assert.IsNotNull(jsonResult);
            var actual = jsonResult.Data as Product;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Can_get_product_by_not_exist_id()
        {
            //Arrange
            var notExistIds = new List<int> { -1, -2, -100, 7, 8, 100 };
            var mock = new Mock<IProductService>();
            var expected = (int)HttpStatusCode.NotFound;
            mock.Setup(m => m.Get(It.IsAny<int>())).Returns((Product)null);
            var controller = new ProductController(mock.Object);

            //Act
            foreach (var notExistId in notExistIds)
            {
                var actual = controller.Get(notExistId) as HttpStatusCodeResult;
                //Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected, actual.StatusCode);
            }
        }

        [TestMethod]
        public void Can_create_product()
        {
            //Arrange
            var product = new Product { Name = "Car yellow", Quantity = 5, Price = 15000 };
            var mock = new Mock<IProductService>();
            var expected = (int)HttpStatusCode.Created;
            mock.Setup(m => m.Create(product));
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Create(product);
            Assert.IsNotNull(actual);

            //Assert
            Assert.AreEqual(expected, actual.StatusCode);
        }

        [TestMethod]
        public void Can_delete_product()
        {
            //Arrange
            const int id = 5;
            var mock = new Mock<IProductService>();
            var expected = (int)HttpStatusCode.NoContent;
            mock.Setup(m => m.Delete(It.IsAny<int>()));
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Delete(id);
            Assert.IsNotNull(actual);

            //Assert
            Assert.AreEqual(expected, actual.StatusCode);
        }

        [TestMethod]
        public void Can_delete_product_with_negative_id()
        {
            //Arrange
            const int id = -5;
            var mock = new Mock<IProductService>();
            var expected = (int)HttpStatusCode.BadRequest;
            mock.Setup(m => m.Delete(It.IsAny<int>()));
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Delete(id);
            Assert.IsNotNull(actual);

            //Assert
            Assert.AreEqual(expected, actual.StatusCode);
        }
    }
}
