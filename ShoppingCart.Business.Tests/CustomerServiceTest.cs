using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.DAL;

namespace ShoppingCart.Business.Tests
{
    [TestClass]
    public class CustomerServiceTest
    {
        [TestMethod]
        public void Can_create_customer()
        {
            var customer = new Customer { Name = "Bob", Email = "bob@rambler.ru", Card = "555555" };
            var mock = new Mock<ICustomerRepository>();
            mock.Setup(m => m.Create(customer));
            var service = new CustomerService(mock.Object);

            service.Create(customer);
        }

        [TestMethod]
        public void Can_update_customer()
        {
            var customer = new Customer { Name = "Bob", Email = "bob@rambler.ru", Card = "555555" };
            var mock = new Mock<ICustomerRepository>();
            mock.Setup(m => m.Update(customer));
            var service = new CustomerService(mock.Object);

            service.Update(customer);
        }
    }
}
