using System;
using System.Collections.Generic;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Business;
using ShoppingCart.DAL;
using ShoppingCart.WebApi.Controllers;

namespace ShoppingCart.WebApi.Tests
{
    [TestClass]
    public class OrderControllerTest
    {
        [TestMethod]
        public void Can_get_list()
        {
            //Arrange
            var list = new List<Order>
            { new Order { Total = 20000, LineItems = new List<LineItem>
                {
                    new LineItem { ProductId = 1, Name = "Car", Price = 10000, Quantity = 2 },

                }},
                new Order
            {
                Total = 20000, LineItems = new List<LineItem>
                {
                    new LineItem { ProductId = 2, Name = "House", Price = 20000, Quantity = 3 }

                }}
            };
            var expected = list;
            var mockOrderService = new Mock<IOrderService>();
            var mockProductService = new Mock<IProductService>();
            mockOrderService.Setup(m => m.List(0, 5)).Returns(list);
            var controller = new OrderController(mockOrderService.Object, mockProductService.Object);

            //Act
            var actual = controller.List(0, 5) as OkNegotiatedContentResult<IList<Order>>;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Content);
        }

        [TestMethod]
        public void Can_get_list_with_incorect_page()
        {
            //Arrange
            var mockOrderService = new Mock<IOrderService>();
            var mockProductService = new Mock<IProductService>();
            var controller = new OrderController(mockOrderService.Object, mockProductService.Object);
            var incorrectPages = new List<int> { -100, -1, 0 };
            //Act
            foreach (var incorrectPage in incorrectPages)
            {
                controller.List(incorrectPage, 5);
                //Assert
                mockOrderService.Verify(ps => ps.List(0, 5));
            }
        }

        [TestMethod]
        public void Can_get_list_with_incorect_pageSize()
        {
            //Arrange
            var mockOrderService = new Mock<IOrderService>();
            var mockProductService = new Mock<IProductService>();
            var controller = new OrderController(mockOrderService.Object, mockProductService.Object);
            var incorrectPageSizes = new List<int> { -1, 0, 251 };
            //Act
            foreach (var incorrectPageSize in incorrectPageSizes)
            {
                controller.List(1, incorrectPageSize);
                //Assert
                mockOrderService.Verify(ps => ps.List(0, 50));
            }
        }

        [TestMethod]
        public void can_get_list_with_exception()
        {
            var mockOrderService = new Mock<IOrderService>();
            var mockProductService = new Mock<IProductService>();
            mockOrderService.Setup(m => m.List(0, 5)).Throws(new Exception());
            var controller = new OrderController(mockOrderService.Object, mockProductService.Object);
            var actual = controller.List(0, 5) as InternalServerErrorResult;
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void Can_get_order_by_id()
        {
            //Arrange
            const int id = 7;
            var mockOrderService = new Mock<IOrderService>();
            var mockProductService = new Mock<IProductService>();
            var lineItem = new List<LineItem> { new LineItem { Name = "Car", Price = 20000, Quantity = 1 } };
            var expected = new Order { Id = id, LineItems = lineItem };
            mockOrderService.Setup(m => m.Get(id)).Returns(expected);
            var controller = new OrderController(mockOrderService.Object, mockProductService.Object);

            //Act
            var actual = controller.GetById(id) as OkNegotiatedContentResult<Order>;
            Assert.IsNotNull(actual);

            //Assert
            Assert.AreEqual(expected, actual.Content);
        }

        [TestMethod]
        public void Can_get_order_by_not_exist_id()
        {
            //Arrange
            var notExistIds = new List<int> { 100, 692 };
            var mockOrderService = new Mock<IOrderService>();
            var mockProductService = new Mock<IProductService>();
            mockOrderService.Setup(m => m.Get(It.IsAny<int>())).Returns((Order)null);
            var controller = new OrderController(mockOrderService.Object, mockProductService.Object);

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
        public void Can_get_order_by_negative_id()
        {
            //Arrange
            var negativeId = new List<int> { -1, -2, -100 };
            var mockOrderService = new Mock<IOrderService>();
            var mockProductService = new Mock<IProductService>();
            mockOrderService.Setup(m => m.Get(It.IsAny<int>())).Returns((Order)null);
            var controller = new OrderController(mockOrderService.Object, mockProductService.Object);

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
        public void can_get_order_by_id_with_exception()
        {
            const int id = 1;
            var mockOrderService = new Mock<IOrderService>();
            var mockProductService = new Mock<IProductService>();
            mockOrderService.Setup(m => m.Get(id)).Throws(new Exception());
            var controller = new OrderController(mockOrderService.Object, mockProductService.Object);

            var actual = controller.GetById(id) as InternalServerErrorResult;

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void Can_place_order()
        {
            //Arrange
            const int id = 1;
            var product = new Product { Id = id, Name = "Car", Price = 20000, Quantity = 3 };
            var lineItem = new List<LineItem> { new LineItem { ProductId = id, Name = "Car", Price = 20000, Quantity = 1 } };
            var order = new Order { Id = 3, LineItems = lineItem };
            const string expectedRouteName = "DefaultApi";
            var expectedContent = order.Id;
            var expectedRouteValue = order.Id;
            var mockOrderService = new Mock<IOrderService>();
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(m => m.Get(It.IsAny<int>())).Returns(product);
            mockOrderService.Setup(m => m.Place(order));
            var controller = new OrderController(mockOrderService.Object, mockProductService.Object);

            //Act
            var actual = controller.Place(order) as CreatedAtRouteNegotiatedContentResult<int>;

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedRouteName, actual.RouteName);
            Assert.AreEqual(expectedContent, actual.Content);
            Assert.AreEqual(expectedRouteValue, actual.RouteValues["id"]);
        }

        [TestMethod]
        public void Can_create_order_null_entity()
        {
            //Arrange
            const string expected = "Entity is not valid";
            var mockOrderService = new Mock<IOrderService>();
            var mockProductService = new Mock<IProductService>();
            var controller = new OrderController(mockOrderService.Object, mockProductService.Object);

            //Act
            var actual = controller.Place(null) as BadRequestErrorMessageResult;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_create_order_with_empty_lineItems()
        {
            //Arrange
            const string expected = "lineItems are empty";
            var order = new Order
            {
                LineItems = null
            };
            var mockOrderService = new Mock<IOrderService>();
            var mockProductService = new Mock<IProductService>();
            var controller = new OrderController(mockOrderService.Object, mockProductService.Object);

            //Act
            var actual = controller.Place(order) as BadRequestErrorMessageResult;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_create_order_with_not_exist_lineItem_id()
        {
            //Arrange
            const int notExistLineItemId = 1000;
            var expected = $"Product with id = {notExistLineItemId} is not exist";

            var order = new Order
            {
                LineItems = new List<LineItem>
                {
                    new LineItem {ProductId = notExistLineItemId, Name = "Car", Price = 20000, Quantity = 4}
                }
            };
            var mockOrderService = new Mock<IOrderService>();
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(m => m.Get(notExistLineItemId)).Returns((Product)null);
            var controller = new OrderController(mockOrderService.Object, mockProductService.Object);

            //Act
            var actual = controller.Place(order) as BadRequestErrorMessageResult;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_create_order_with_lineItemsQuantity_less_then_zero()
        {
            //Arrange
            const int id = 1;
            var product = new Product { Id = 1, Name = "Car", Price = 20000, Quantity = 3 };
            const string expected = "Quantity can't be less then 0";

            var order = new Order
            {
                LineItems = new List<LineItem>
                {
                    new LineItem {ProductId = id, Name = "Car", Price = 20000, Quantity = -1}
                }
            };
            var mockOrderService = new Mock<IOrderService>();
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(m => m.Get(id)).Returns(product);
            var controller = new OrderController(mockOrderService.Object, mockProductService.Object);

            //Act
            var actual = controller.Place(order) as BadRequestErrorMessageResult;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Message);
        }

        [TestMethod]
        public void Can_create_order_with_exception()
        {
            //Arrange
            const int id = 1;
           var order = new Order
            {
                LineItems = new List<LineItem>
                {
                    new LineItem {ProductId = id, Name = "Car", Price = 20000, Quantity = 3}
                }
            };
            var mockOrderService = new Mock<IOrderService>();
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(m => m.Get(id)).Throws(new Exception());
            var controller = new OrderController(mockOrderService.Object, mockProductService.Object);

            //Act
            var actual = controller.Place(order) as InternalServerErrorResult;
            Assert.IsNotNull(actual);
        }

    }
}
