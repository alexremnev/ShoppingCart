using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Business;
using ShoppingCart.DAL;
using ShoppingCart.DAL.NHibernate;
using ShoppingCart.WebApi.Controllers;
using ShoppingCart.WebApi.Security;

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
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(m => m.List(null, null, true, 0, 5, null)).Returns(list);
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

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
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);
            var incorrectPages = new List<int> { -100, -1, 0 };
            //Act
            foreach (var incorrectPage in incorrectPages)
            {
                controller.List(null, null, null, incorrectPage, 5);
                //Assert
                mockProductService.Verify(ps => ps.List(null, null, true, 0, 5, null));
            }
        }

        [TestMethod]
        public void Can_get_list_with_incorect_pageSize()
        {
            //Arrange
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);
            var incorrectPageSizes = new List<int> { -1, 0, 251 };
            //Act
            foreach (var incorrectPageSize in incorrectPageSizes)
            {
                controller.List(null, null, null, 1, incorrectPageSize);
                //Assert
                mockProductService.Verify(ps => ps.List(null, null, true, 0, 50, null));
            }
        }

        [TestMethod]
        public void Can_get_list_with_incorrect_filter()
        {
            //Arrenge
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockProductService.Setup(m => m.List(null, null, true, 1, 5, null)).Returns((IList<Product>)null);
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);
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
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            const int count = 7;
            var expected = count;
            mockProductService.Setup(m => m.Count(It.IsAny<string>(), 0)).Returns(count);
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

            //Act
            var actual = controller.Count() as OkNegotiatedContentResult<int>;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Content);
            mockProductService.Verify(ps => ps.Count(null, 0));
        }

        [TestMethod]
        public void Can_get_product_by_id()
        {
            //Arrange
            const int id = 7;
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var expected = new Product { Id = id, Name = "Car yellow", Quantity = 5, Price = 15000 };
            mockProductService.Setup(m => m.Get(id)).Returns(expected);
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

            //Act
            var claims = new List<Claim>//todo:delete it
                {
                    new Claim(ClaimTypes.Name,"Bob"),
                    new Claim(ClaimTypes.Role, "class-mate")
                };
            IPrincipal principal = new ClaimsPrincipal(new[] { new ClaimsIdentity(claims, "Token") });
            Thread.CurrentPrincipal = principal;
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
            var mockProductSrvice = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockProductSrvice.Setup(m => m.Get(It.IsAny<int>())).Returns((Product)null);
            var controller = new ProductController(mockProductSrvice.Object, mockSecurityContext.Object);

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
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockProductService.Setup(m => m.Get(It.IsAny<int>())).Returns((Product)null);
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

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
            const string expectedRouteName = "DefaultApi";
            var expectedContent = product.Id;
            var expectedRouteValue = product.Id;
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockProductService.Setup(m => m.GetByName(product.Name)).Returns((Product)null);
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

            //Act
            var actual = controller.Create(product) as CreatedAtRouteNegotiatedContentResult<int>;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedRouteName, actual.RouteName);
            Assert.AreEqual(expectedContent, actual.Content);
            Assert.AreEqual(expectedRouteValue, actual.RouteValues["id"]);
        }

        [TestMethod]
        public void Can_create_product_null_entity()
        {
            //Arrange
            const string expected = "Entity is not valid";
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

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
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

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
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

            //Act
            var actual = controller.Create(product) as BadRequestErrorMessageResult;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_create_product_with_not_unique_name()
        {
            //Arrange
            var product = new Product { Name = "Car" };
            const string expected = "Name is not unique";
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockProductService.Setup(m => m.GetByName(product.Name)).Returns(new Product { Name = "Car" });
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

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
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockProductService.Setup(m => m.GetByName(product.Name)).Throws(new Exception());
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

            //Act
            var actual = controller.Create(product) as InternalServerErrorResult;
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void Can_update_product()
        {
            //Arrange
            var product = new Product { Id = 5, Name = "Car yellow", Quantity = 5, Price = 15000 };
            var mockproductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var expected = HttpStatusCode.NoContent;
            mockproductService.Setup(m => m.GetByName(product.Name)).Returns((Product)null);
            mockproductService.Setup(m => m.Update(product));
            mockproductService.Setup(m => m.Get(It.IsAny<int>())).Returns(product);
            var controller = new ProductController(mockproductService.Object, mockSecurityContext.Object);

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
            var value = new Product { Id = 5, Name = "Car" };
            var product = new Product { Id = 5, Name = "Car" };
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockProductService.Setup(m => m.GetByName(product.Name)).Returns(value);
            mockProductService.Setup(m => m.Get(It.IsAny<int>())).Returns(value);
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

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
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

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
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

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
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

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
            var value = new Product { Id = 4, Name = "Car" };
            const string expected = "Name is not unique";
            var product = new Product { Name = "Car" };
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockProductService.Setup(m => m.GetByName(product.Name)).Returns(value);
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

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
            var value = new Product { Id = 3, Name = "Car" };
            const string expected = "Name is not unique";
            var product = new Product { Id = 5, Name = "Car" };
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockProductService.Setup(m => m.GetByName(product.Name)).Returns(value);
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

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
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockProductService.Setup(m => m.GetByName(product.Name)).Throws(new Exception());
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

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
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var expected = HttpStatusCode.NoContent;
            mockProductService.Setup(m => m.Delete(It.IsAny<int>()));
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

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
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockProductService.Setup(m => m.Delete(It.IsAny<int>()));
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object);

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


        [TestMethod]
        public void Test()
        {
            // Arrange
            //  ProductsController controller = new ProductsController(repository);
            const int id = 7;
            var mockProductService = new Mock<IProductService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var expected = new Product { Id = id, Name = "Car yellow", Quantity = 5, Price = 15000 };
            mockProductService.Setup(m => m.Get(id)).Returns(expected);
            var controller = new ProductController(mockProductService.Object, mockSecurityContext.Object)
            {
                Request = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://localhost/api/product/7")
                },
                Configuration = new HttpConfiguration()
            };
            //controller.Configuration.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional });

            //controller.RequestContext.RouteData = new HttpRouteData(
            //    route: new HttpRoute(),
            //    values: new HttpRouteValueDictionary { { "controller", "product", "1" } });

            // Act
            var claims = new List<Claim>//todo:delete it
                {
                    new Claim(ClaimTypes.Name,"Bob"),
                    new Claim(ClaimTypes.Role, "class-mate")
                };
            IPrincipal principal = new ClaimsPrincipal(new[] { new ClaimsIdentity(claims, "Token") });
            Thread.CurrentPrincipal = principal;
            controller.RequestContext.Principal = principal;
            var response = controller.GetById(7) as UnauthorizedResult;

            // Assert
            Assert.IsNotNull(response);

        }

    }
}

