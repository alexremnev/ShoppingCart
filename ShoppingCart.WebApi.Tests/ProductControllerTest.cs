using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web.Http.Results;
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
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.List(null, null, true, 0, 5)).Returns(list);
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Get(null, null, true, 1, 5) as OkNegotiatedContentResult<IList<Product>>;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Content);
        }

        [TestMethod]
        public void Can_get_list_with_incorect_page()
        {
            //Arrange
            var mock = new Mock<IProductService>();
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
            var mock = new Mock<IProductService>();
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
        public void Can_get_list_with_incorrect_filter()
        {
            //Arrenge
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.List(null, null, true, 1, 5)).Returns((IList<Product>)null);
            var controller = new ProductController(mock.Object);
            const string notExistentFilter = "notExistentFilter";

            //Act
            var actual = controller.Get(notExistentFilter, null, true, 1, 5) as OkNegotiatedContentResult<IList<Product>>;

            //Assert
            Assert.IsNotNull(actual);
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
            var actual = controller.Count(null) as OkNegotiatedContentResult<int>;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Content);
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

            //Act
            var actual = controller.Get(id) as OkNegotiatedContentResult<Product>;
            Assert.IsNotNull(actual);

            //Assert
            Assert.AreEqual(expected, actual.Content);
        }

        [TestMethod]
        public void Can_get_product_by_not_exist_id()
        {
            //Arrange
            var notExistIds = new List<int> { 7, 8, 100, 692 };
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.Get(It.IsAny<int>())).Returns((Product)null);
            var controller = new ProductController(mock.Object);

            //Act
            foreach (var notExistId in notExistIds)
            {
                var actual = controller.Get(notExistId);
                //Assert
                Assert.IsNotNull(actual);
                Assert.IsInstanceOfType(actual, typeof(NotFoundResult));
            }
        }

        [TestMethod]
        public void Can_get_product_by_negative_id()
        {
            //Arrange
            var negativeId = new List<int> { -1, -2, -100 };
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.Get(It.IsAny<int>())).Returns((Product)null);
            var controller = new ProductController(mock.Object);

            //Act
            foreach (var id in negativeId)
            {
                var actual = controller.Get(id);
                //Assert
                Assert.IsNotNull(actual);
                Assert.IsInstanceOfType(actual, typeof(BadRequestResult));
            }
        }

        [TestMethod]
        public void Can_create_product()
        {
            //Arrange
            var product = new Product { Name = "Car yellow", Quantity = 5, Price = 15000 };
            var expected = product.Id;
            var expectedLocation = new Uri($"http://localhost:50896/api/product/{expected}");
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.Create(product));
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Post(product) as CreatedNegotiatedContentResult<int>;
            
            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Content);
            Assert.AreEqual(expectedLocation, actual.Location);
        }

        [TestMethod]
        public void Can_create_product_with_long_name()
        {
            //Arrange
            var product = new Product { Name = GenerateName(50), Quantity = 5, Price = 15000 };
            var mock = new Mock<IProductService>();
            var exception = new ValidationException("Name consists of more than 50 letters");
            mock.Setup(m => m.Create(product)).Throws(exception);
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Post(product) as BadRequestErrorMessageResult;
            Assert.IsNotNull(actual);
            Assert.AreEqual(exception.Message, actual.Message);
        }

        [TestMethod]
        public void Can_create_product_with_not_unique_name()
        {
            //Arrange
            var product = new Product {Name = "Car"};
            var mock = new Mock<IProductService>();
            var exception = new ValidationException("Name is not unique");
            mock.Setup(m => m.Create(product)).Throws(exception);
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Post(product) as BadRequestErrorMessageResult;
            Assert.IsNotNull(actual);
            Assert.AreEqual(exception.Message, actual.Message);
        }

        [TestMethod]
        public void Can_update_product()
        {
            //Arrange
            const int id = 5;
            var product = new Product() { Name = "Car yellow", Quantity = 5, Price = 15000 };
            var mock = new Mock<IProductService>();
            var expected = HttpStatusCode.NoContent;
            mock.Setup(m => m.Update(id,product)).Returns(true);
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Put(id,product) as StatusCodeResult;
            
            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.StatusCode);
        }

        [TestMethod]
        public void Can_update_product_with_negative_id()
        {
            //Arrange
            const int id = -5;
            var product = new Product() { Name = "Car yellow", Quantity = 5, Price = 15000 };
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.Update(id, product)).Returns(true);
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Put(id, product) as BadRequestResult;
            
            //Assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void Can_update_with_wrong_product()
        {
            //Arrange
            const int id = 5;
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.Update(id, null)).Returns(false);
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Put(id, null) as BadRequestResult;

            //Assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void Can_update_product_with_not_unique_name()
        {
            //Arrange
            var product = new Product { Name = "Car" };
            var mock = new Mock<IProductService>();
            var exception = new ValidationException("Name is not unique");
            mock.Setup(m => m.Update(product.Id,product)).Throws(exception);
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Put(product.Id,product) as BadRequestErrorMessageResult;
            Assert.IsNotNull(actual);
            Assert.AreEqual(exception.Message, actual.Message);
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
            var actual = controller.Delete(id) as StatusCodeResult;
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
            mock.Setup(m => m.Delete(It.IsAny<int>()));
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Delete(id) as BadRequestResult;

            //Assert
            Assert.IsNotNull(actual);
        }

        private static string GenerateName(int number)
        {
            var name = new StringBuilder();
            for (var i = 0; i <= number; i++)
            {
                name.Append('a');
            }
            return name.ToString();
        }
    }
}

