using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.DAL;
using ShoppingCart.ProductService;
using ShoppingCart.WebApi.Controllers;

namespace ShoppingCart.WebApi.Tests
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
                        new Product {Id = 4, Name = "Apple anotherType", Quantity = 5, Price = 37},
                        new Product {Id = 5, Name = "apple", Quantity = 25, Price = 40}
                    };
            var expected = list;
            var expectedStatusCode = HttpStatusCode.OK;
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.Count(null)).Returns(list.Count);
            mock.Setup(m => m.List(null, null, true, 0, 5)).Returns(list);
            var controller = new ProductController(mock.Object);

            //Act
            var result = controller.Get(null, null, true, 1, 5);
            Assert.IsNotNull(result);
            var actual = result.Content.ReadAsAsync<List<Product>>().Result;
            var actualStatusCode = result.StatusCode;

            //Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
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
                controller.Get(null, null, null, incorrectPage, 5);
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
                controller.Get(null, null, null, 1, incorrectPageSize);
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
            var expectedStatusCode = HttpStatusCode.OK;

            //Act
            var result = controller.Count(null);

            //Assert
            Assert.IsNotNull(result);
            var actual = result.Content.ReadAsAsync<int>().Result;
            var actualStatusCode = result.StatusCode;
            Assert.IsInstanceOfType(actual, typeof(int));
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
            mock.Verify(ps => ps.Count(null));
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
            var expectedStatusCode = HttpStatusCode.OK;
            //Act
            var result = controller.Get(id);
            Assert.IsNotNull(result);
            var actual = result.Content.ReadAsAsync<Product>().Result;
            var actualStatusCode = result.StatusCode;

            //Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
        }

        [TestMethod]
        public void Can_get_product_by_not_exist_id()
        {
            //Arrange
            var notExistIds = new List<int> { -1, -2, -100, 7, 8, 100 };
            var mock = new Mock<IProductService>();
            var expected = HttpStatusCode.NotFound;
            mock.Setup(m => m.Get(It.IsAny<int>())).Returns((Product)null);
            var controller = new ProductController(mock.Object);

            //Act
            foreach (var notExistId in notExistIds)
            {
                var actual = controller.Get(notExistId).StatusCode;
                //Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void Can_create_product()
        {
            //Arrange
            var product = new Product { Name = "Car yellow", Quantity = 5, Price = 15000 };
            var mock = new Mock<IProductService>();
            var expected = HttpStatusCode.Created;
            mock.Setup(m => m.Create(product));
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Post(product);
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
            var expected = HttpStatusCode.NoContent;
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
            var expected = HttpStatusCode.BadRequest;
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

