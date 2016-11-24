using System;
using System.Collections.Generic;
using Common.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Spring.Testing.Microsoft;

namespace ShoppingCart.DAL.NHibernate.Tests
{

    [TestClass]
    public class CustomerRepositoryTest : AbstractTransactionalSpringContextTests
    {
       
        public ISessionFactory Sessionfactory { get; set; }
        public CustomerRepository CustomerRepository { get; set; }

        private static readonly ILog Log = LogManager.GetLogger<CustomerRepositoryTest>();

        private void CreateInitialData(IList<Customer> customers)
        {
            var initialList = customers;

            using (var session = Sessionfactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                foreach (var customer in initialList)
                    session.Save(customer);
                transaction.Commit();
            }
        }

        [TestInitialize]
        public void SetupContext()
        {
            Sessionfactory.OpenSession().CreateQuery("DELETE FROM Customer ").ExecuteUpdate();
        }

        [TestMethod]
        public void Can_create_new_customer()
        {
            var expected = new Customer { Name = "Bob", Email = "bob@rambler.ru", Card = "555555" };
            var repository = CustomerRepository;

            repository.Create(expected);
            Sessionfactory.GetCurrentSession().Transaction.Commit();
            Sessionfactory.GetCurrentSession().Flush();

            using (var session = Sessionfactory.OpenSession())
            {
                var actual = session.Get<Customer>(expected.Id);

                Assert.IsNotNull(actual);
                Assert.AreNotSame(expected, actual);
                CompareCustomers(expected, actual);
            }

        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Can_create_product_with_null_entity()
        {
            var repository = CustomerRepository;
            repository.Create(null);
        }

        private static void CompareCustomers(Customer expected, Customer actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Email, actual.Email);
            Assert.AreEqual(expected.Card, actual.Card);
        }

        [TestMethod]
        public void Can_update_product()
        {
            var customer = new Customer { Name = "Bob", Email = "bob@rambler.ru", Card = "555555" };
            CreateInitialData(new List<Customer> { customer });
            var expected = new Customer { Id = customer.Id, Name = "Clark", Email = "clark@rambler.ru", Card = "111111" };
            var repository = CustomerRepository;

            repository.Update(expected);

            Sessionfactory.GetCurrentSession().Transaction.Commit();
            Sessionfactory.GetCurrentSession().Flush();
            using (var session = Sessionfactory.OpenSession())
            {
                var actual = session.Get<Customer>(customer.Id);
                Assert.AreEqual(expected.Name, actual.Name);
                Assert.AreEqual(expected.Email, actual.Email);
                Assert.AreEqual(expected.Card, actual.Card);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Can_update_product_with_null_entity()
        {

            var repository = CustomerRepository;

            repository.Update(null);
        }

        protected override string[] ConfigLocations => new[]
       {
            "config://spring/objects",
            "assembly://ShoppingCart.DAL.NHibernate/ShoppingCart.DAL.NHibernate/config.xml"
        };
    }
}
