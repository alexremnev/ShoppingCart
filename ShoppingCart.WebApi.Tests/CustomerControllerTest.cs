using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Business;
using ShoppingCart.DAL;
using ShoppingCart.DAL.NHibernate;
using ShoppingCart.WebApi.Controllers;

namespace ShoppingCart.WebApi.Tests
{
    [TestClass]
    public class CustomerControllerTest
    {
        [TestMethod]
        public void Can_get_list()
        {
            //Arrange
            IList<Customer> list = new List<Customer>
                    {
                        new Customer {Id = 1, Name = "Bob", Email="Bob@rambler.ru"},
                        new Customer {Id = 2, Name = "Mark", Email="Mark@rambler.ru"}

                        };
            var expected = list;
            var mockCustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockCustomerService.Setup(m => m.List(null, null, true, 0, 5,null)).Returns(list);
            var controller = new CustomerController(mockCustomerService.Object, mockSecurityContext.Object);

            //Act
            var actual = controller.List(0, 5) as OkNegotiatedContentResult<IList<Customer>>;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Content);
        }

        [TestMethod]
        public void Can_get_list_with_incorect_page()
        {
            //Arrange
            var mockCustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new CustomerController(mockCustomerService.Object, mockSecurityContext.Object);
            var incorrectPages = new List<int> { -100, -1, 0 };
            //Act
            foreach (var incorrectPage in incorrectPages)
            {
                controller.List(incorrectPage, 5);
                //Assert
                mockCustomerService.Verify(ps => ps.List(null, null, true, 0, 5,null));
            }
        }

        [TestMethod]
        public void Can_get_list_with_incorect_pageSize()
        {
            //Arrange
            var mockcustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new CustomerController(mockcustomerService.Object, mockSecurityContext.Object);
            var incorrectPageSizes = new List<int> { -1, 0, 251 };
            //Act
            foreach (var incorrectPageSize in incorrectPageSizes)
            {
                controller.List(1, incorrectPageSize);
                //Assert
                mockcustomerService.Verify(ps => ps.List(null, null, true, 0, 50,null));
            }
        }

        [TestMethod]
        public void can_get_list_with_exception()
        {
            var mockCustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockCustomerService.Setup(m => m.List(null, null, true, 0, 5,null)).Throws(new Exception());
            var controller = new CustomerController(mockCustomerService.Object, mockSecurityContext.Object);
            var actual = controller.List(0, 5) as InternalServerErrorResult;
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void Can_get_customer_by_id()
        {
            //Arrange
            const int id = 7;
            var mockCustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var expected = new Customer { Id = id, Name = "Bob", Email = "Bob@rambler.ru" };
            mockCustomerService.Setup(m => m.Get(id)).Returns(expected);
            var controller = new CustomerController(mockCustomerService.Object, mockSecurityContext.Object);

            //Act
            var actual = controller.GetById(id) as OkNegotiatedContentResult<Customer>;
            Assert.IsNotNull(actual);

            //Assert
            Assert.AreEqual(expected, actual.Content);
        }

        [TestMethod]
        public void Can_get_customer_by_not_exist_id()
        {
            //Arrange
            var notExistIds = new List<int> { 100, 692 };
            var mockCustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockCustomerService.Setup(m => m.Get(It.IsAny<int>())).Returns((Customer)null);
            var controller = new CustomerController(mockCustomerService.Object, mockSecurityContext.Object);

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
        public void Can_get_customer_by_negative_id()
        {
            //Arrange
            var negativeId = new List<int> { -1, -2, -100 };
            var mockCustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockCustomerService.Setup(m => m.Get(It.IsAny<int>())).Returns((Customer)null);
            var controller = new CustomerController(mockCustomerService.Object, mockSecurityContext.Object);

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
        public void can_get_customer_by_id_with_exception()
        {
            const int id = 1;
            var mockCustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockCustomerService.Setup(m => m.Get(id)).Throws(new Exception());
            var controller = new CustomerController(mockCustomerService.Object, mockSecurityContext.Object);

            var actual = controller.GetById(id) as InternalServerErrorResult;

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void Can_create_customer()
        {
            //Arrange
            var customer = new Customer { Id = 3, Name = "Bob", Email = "bob@rambler.ru", Card = "555555" };
            var expected = customer.Id;
            var expectedRouteName = "DefaultApi";
            var expectedRouteValue = customer.Id;
            var mockCustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new CustomerController(mockCustomerService.Object, mockSecurityContext.Object);

            //Act
            var actual = controller.Create(customer) as CreatedAtRouteNegotiatedContentResult<int>;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Content);
            Assert.AreEqual(expectedRouteName, actual.RouteName);
            Assert.AreEqual(expectedRouteValue, actual.RouteValues["id"]);
        }

        [TestMethod]
        public void Can_create_customer_null_entity()
        {
            //Arrange
            const string expected = "Entity is not valid";
            var mockCustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new CustomerController(mockCustomerService.Object, mockSecurityContext.Object);

            //Act
            var actual = controller.Create(null) as BadRequestErrorMessageResult;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_create_customer_with_empty_name()
        {
            //Arrange
            var customer = new Customer { Name = "", Email = "bob@rambler.ru", Card = "555555" };
            const string expected = "Name is empty";
            var mockCustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new CustomerController(mockCustomerService.Object, mockSecurityContext.Object);

            //Act
            var actual = controller.Create(customer) as BadRequestErrorMessageResult;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_create_customer_with_exception()
        {
            //Arrange
            var customer = new Customer { Name = "Bob", Email = "bob@rambler.ru", Card = "555555" };
            var mockCustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockCustomerService.Setup(m => m.Create(customer)).Throws(new Exception());
            var controller = new CustomerController(mockCustomerService.Object, mockSecurityContext.Object);

            //Act
            var actual = controller.Create(customer) as InternalServerErrorResult;
            Assert.IsNotNull(actual);
        }
        [TestMethod]
        public void Can_update_customer()
        {
            //Arrange
            var customer = new Customer { Id = 2, Name = "Bob", Email = "bob@rambler.ru", Card = "555555" };
            var mockCustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var expected = HttpStatusCode.NoContent;
            mockCustomerService.Setup(m => m.Update(customer));
            mockCustomerService.Setup(m => m.Get(customer.Id)).Returns(new Customer());
            var controller = new CustomerController(mockCustomerService.Object, mockSecurityContext.Object);

            //Act
            var actual = controller.Update(customer) as StatusCodeResult;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.StatusCode);
        }

        [TestMethod]
        public void Can_update_customer_with_null_entity()
        {
            //Arrange
            const string expected = "Entity is not valid";
            var mockCustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new CustomerController(mockCustomerService.Object, mockSecurityContext.Object);

            //Act
            var actual = controller.Update(null) as BadRequestErrorMessageResult;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_update_customer_with_empty_name()
        {
            //Arrange
            var customer = new Customer { Name = "", Email = "bob@rambler.ru", Card = "555555" };
            const string expected = "Name is empty";
            var mockCustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            var controller = new CustomerController(mockCustomerService.Object, mockSecurityContext.Object);

            //Act
            var actual = controller.Update(customer) as BadRequestErrorMessageResult;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_update_customer_with_exception()
        {
            //Arrange
            var customer = new Customer { Name = "Bob", Email = "bob@rambler.ru", Card = "555555" };
            var mockCustomerService = new Mock<ICustomerService>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            mockSecurityContext.Setup(m => m.UserName).Returns("NewName");
            mockCustomerService.Setup(m => m.Get(customer.Id)).Throws(new Exception());
            var controller = new CustomerController(mockCustomerService.Object, mockSecurityContext.Object);

            //Act
            var actual = controller.Update(customer) as InternalServerErrorResult;

            //Assert
            Assert.IsNotNull(actual);
        }
    }
}

