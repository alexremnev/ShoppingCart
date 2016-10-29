using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Business;
using ShoppingCart.DAL;
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
            var actual = controller.List(null, null, true, 0, 5) as OkNegotiatedContentResult<IList<Product>>;

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
                controller.List(null, null, null, incorrectPage, 5);
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
                controller.List(null, null, null, 1, incorrectPageSize);
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
            var actual = controller.List(notExistentFilter, null, true, 1, 5) as OkNegotiatedContentResult<IList<Product>>;

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
            var actual = controller.Count() as OkNegotiatedContentResult<int>;

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
            var actual = controller.GetById(id) as OkNegotiatedContentResult<Product>;
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
                var actual = controller.GetById(notExistId);
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
                var actual = controller.GetById(id);
                //Assert
                Assert.IsNotNull(actual);
                Assert.IsInstanceOfType(actual, typeof(BadRequestResult));
            }
        }

        [TestMethod]
        public void Can_create_product()
        {
            //Arrange
            var product = new Product { Id = 3, Name = "Car red", Quantity = 5, Price = 15000 };
            var expected = product.Id;
            var expectedRouteName = "DefaultApi";
            var expectedContent = product.Id;
            var expectedRouteValue = product.Id;
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.GetByName(product.Name)).Returns(new List<Product>());
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Create(product) as CreatedAtRouteNegotiatedContentResult<int>;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Content);
            Assert.AreEqual(expectedRouteName, actual.RouteName);
            Assert.AreEqual(expectedContent, actual.Content);
            Assert.AreEqual(expectedRouteValue, actual.RouteValues["id"]);
        }

        [TestMethod]
        public void Can_create_product_null_entity()
        {
            //Arrange
            const string expected = "Entity is not valid";
            var mock = new Mock<IProductService>();
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Create(null) as BadRequestErrorMessageResult;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_create_product_with_empty_name()
        {
            //Arrange
            var product = new Product { Name = "", Quantity = 5, Price = 15000 };
            const string expected = "Name is empty";
            var mock = new Mock<IProductService>();
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Create(product) as BadRequestErrorMessageResult;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_create_product_with_long_name()
        {
            //Arrange
            var product = new Product { Name = GenerateName(50), Quantity = 5, Price = 15000 };
            const string expected = "Name consists of more than 50 letters";
            var mock = new Mock<IProductService>();
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Create(product) as BadRequestErrorMessageResult;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_create_product_with_not_unique_name()
        {
            //Arrange
            var list = new List<Product> { new Product { Name = "Car" } };
            var product = new Product { Name = "Car" };
            const string expected = "Name is not unique";
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.GetByName(product.Name)).Returns(list);
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Create(product) as BadRequestErrorMessageResult;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_create_product_with_exception()
        {
            //Arrange
            var product = new Product { Name = "Car" };
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.GetByName(product.Name)).Throws(new Exception());
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Create(product) as InternalServerErrorResult;
            Assert.IsNotNull(actual);

        }

        [TestMethod]
        public void Can_update_product()
        {
            //Arrange
            var product = new Product() { Id = 5, Name = "Car yellow", Quantity = 5, Price = 15000 };
            var mock = new Mock<IProductService>();
            var expected = HttpStatusCode.NoContent;
            mock.Setup(m => m.GetByName(product.Name)).Returns(new List<Product>());
            mock.Setup(m => m.Update(product));
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Update(product) as StatusCodeResult;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.StatusCode);
        }

        [TestMethod]
        public void Can_update_product_with_not_unique_name_but_same_id()
        {
            //Arrange
            var expected = HttpStatusCode.NoContent;
            var listCar = new List<Product> { new Product { Id = 5, Name = "Car" } };
            var product = new Product { Id = 5, Name = "Car" };
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.GetByName(product.Name)).Returns(listCar);
            // mock.Setup(m => m.Update(product));
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Update(product) as StatusCodeResult;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.StatusCode);
        }

        [TestMethod]
        public void Can_update_with_null_entity()
        {
            //Arrange
            const string expected = "Entity is not valid";
            var mock = new Mock<IProductService>();
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Update(null) as BadRequestErrorMessageResult;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_update_with_empty_name()
        {
            //Arrange
            var product = new Product { Name = "", Price = 20, Quantity = 50 };
            const string expected = "Name is empty";
            var mock = new Mock<IProductService>();
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Update(product) as BadRequestErrorMessageResult;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_update_product_with_long_name()
        {
            //Arrange
            var product = new Product { Name = GenerateName(50), Price = 20, Quantity = 50 };
            const string expected = "Name consists of more than 50 letters";
            var mock = new Mock<IProductService>();
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Update(product) as BadRequestErrorMessageResult;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }


        [TestMethod]
        public void Can_update_product_with_not_unique_name()
        {
            //Arrange
            var listCar = new List<Product> { new Product { Name = "Car" }, new Product { Name = "Car" } };
            const string expected = "Name is not unique";
            var product = new Product { Name = "Car" };
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.GetByName(product.Name)).Returns(listCar);
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Update(product) as BadRequestErrorMessageResult;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_update_product_with_not_unique_name_but_not_same_id()
        {
            //Arrange
            var listCar = new List<Product> { new Product { Id = 3, Name = "Car" } };
            const string expected = "Name is not unique";
            var product = new Product { Id = 5, Name = "Car" };
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.GetByName(product.Name)).Returns(listCar);
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Update(product) as BadRequestErrorMessageResult;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }


        [TestMethod]
        public void Can_update_with_exception()
        {
            //Arrange
            var product = new Product { Id = 5, Name = "Car" };
            var mock = new Mock<IProductService>();
            mock.Setup(m => m.GetByName(product.Name)).Throws(new Exception());
            var controller = new ProductController(mock.Object);

            //Act
            var actual = controller.Update(product) as InternalServerErrorResult;

            //Assert
            Assert.IsNotNull(actual);
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

