using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.DAL;

namespace ShoppingCart.Business.Tests
{
    [TestClass]
    public class OrderServiceTest
    {
        [TestMethod]
        public void Can_get_list()
        {
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
            var mockRepository = new Mock<IOrderRepository>();
            var mockService = new Mock<IProductService>();
            mockRepository.Setup(m => m.List(null, null, true, 0, 50,null)).Returns(list);
            var service = new OrderService(mockRepository.Object, mockService.Object);

            var actual = service.List(null, null, true, 0, 50);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Can_get_order_by_id()
        {
            const int id = 7;
            var lineItem = new List<LineItem> { new LineItem { Name = "Car", Price = 20000, Quantity = 1 } };
            var expected = new Order { Id = id, LineItems = lineItem };
            var mockRepository = new Mock<IOrderRepository>();
            var mockService = new Mock<IProductService>();
            mockRepository.Setup(m => m.Get(id)).Returns(expected);
            var service = new OrderService(mockRepository.Object, mockService.Object);

            var actual = service.Get(id);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Can_place_order()
        {
            const int id = 7;
            const int productId = 3;
            var product = new Product { Id = productId, Name = "Car", Price = 20000, Quantity = 3 };
            var lineItem = new List<LineItem> { new LineItem { ProductId = productId, Name = "Car", Price = 20000, Quantity = 1 } };
            var order = new Order { Id = id, LineItems = lineItem };
            var mockRepository = new Mock<IOrderRepository>();
            var mockService = new Mock<IProductService>();
            mockService.Setup(m => m.Get(productId)).Returns(product);
            mockService.Setup(m => m.Update(product));
            mockRepository.Setup(m => m.Create(order));

            var service = new OrderService(mockRepository.Object, mockService.Object);

            service.Place(order);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void can_place_order_with_null_entity()
        {
            var mockRepository = new Mock<IOrderRepository>();
            var mockService = new Mock<IProductService>();

            var service = new OrderService(mockRepository.Object, mockService.Object);

            service.Place(null);
        }
    }
}
