using System;
using System.Collections.Generic;
using Common.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Spring.Testing.Microsoft;

namespace ShoppingCart.DAL.NHibernate.Tests
{
    [TestClass]
    public class OrderRepositoryTest : AbstractTransactionalSpringContextTests
    {
        public ISessionFactory Sessionfactory { get; set; }
        public OrderRepository OrderRepository { get; set; }
        private static readonly ILog Log = LogManager.GetLogger<OrderRepositoryTest>();


        private void CreateInitialData(IList<Order> orders)
        {
            var initialList = orders;

            using (var session = Sessionfactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                foreach (var order in initialList)
                    session.Save(order);
                transaction.Commit();
            }
        }

        [TestInitialize]
        public void SetupContext()
        {
            var repository = OrderRepository;
            var list = repository.List();
            if (list == null) return;
            using (var session = Sessionfactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                foreach (var order in list)
                    session.Delete(order);
                transaction.Commit();
            }
        }

        [TestMethod]
        public void Can_create_order()
        {
            var expected = new Order { Total = 20000, LineItems = new List<LineItem> { new LineItem { ProductId = 2, Name = "Car", Price = 10000, Quantity = 2 } } };
            var repository = OrderRepository;

            repository.Create(expected);
            Sessionfactory.GetCurrentSession().Transaction.Commit();
            Sessionfactory.GetCurrentSession().Flush();
            using (var session = Sessionfactory.OpenSession())
            {
                var actual = session.Get<Order>(expected.Id);

                Assert.IsNotNull(actual);
                Assert.AreNotSame(expected, actual);
                CompareOrders(expected, actual);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Can_create_order_with_null_entity()
        {
            var repository = OrderRepository;
            repository.Create(null);
        }

        [TestMethod]
        public void Can_get_list_of_orders()
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
            CreateInitialData(list);
            var expected = list;
            var repository = OrderRepository;

            var actual = repository.List();

            CompareOrderLists(expected, actual);
        }

        private static void CompareOrderLists(IList<Order> expected, IList<Order> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                CompareOrders(expected[i], actual[i]);
            }
        }

        private static void CompareOrders(Order expected, Order actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Total, actual.Total);
          
            for (var i = 1; i < expected.LineItems.Count; i++)
            {
                Assert.AreEqual(expected.LineItems[i],actual.LineItems[i]);
            }
        }

        protected override string[] ConfigLocations => new[]
       {
            "config://spring/objects",
            "assembly://ShoppingCart.DAL.NHibernate/ShoppingCart.DAL.NHibernate/config.xml"
        };
    }
}
