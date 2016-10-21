using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.DAL;

namespace ShoppingCart.Tests
{
    [TestClass]
    public class ProductServiceTest
    {
        [TestMethod]
        public void Can_get_list()
        {
            IList<Product> list = new List<Product>
        {
            new Product {Id = 1, Name = "Car yellow", Quantity = 5, Price = 15000},
            new Product {Id = 2, Name = "car blue", Quantity = 7, Price = 20000},
            new Product {Id = 3, Name = "apple oneType", Quantity = 3, Price = 40},
            new Product {Id = 5, Name = "Apple anotherType", Quantity = 5, Price = 37},
            new Product {Id = 5, Name = "apple", Quantity = 25, Price = 40}
        };
            var expected = list;
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.List(null, null, true, 0, 50)).Returns(list);
            var service = new ProductService(mock.Object);

            var actual = service.List(null, null, true, 0, 50);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Can_get_product_by_id()
        {
            const int id = 7;
            var expected = new Product { Id = id, Name = "Car yellow", Quantity = 5, Price = 15000 };
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Get(id)).Returns(expected);
            var service = new ProductService(mock.Object);

            var actual = service.Get(id);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Can_can_create_product()
        {
            const int id = 7;
            var product = new Product { Id = id, Name = "Car yellow", Quantity = 5, Price = 15000 };
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Create(product));
            var service = new ProductService(mock.Object);

            service.Create(product);
        }

        [TestMethod]
        public void Can_delete_product_by_id()
        {
            const int id = 7;
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Delete(id));
            var service = new ProductService(mock.Object);

            service.Delete(id);
        }

        [TestMethod]
        public void Can_get_count()
        {
            var expected = 5;
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Count(null)).Returns(expected);
            var service = new ProductService(mock.Object);


            var actual = service.Count(null);
            Assert.AreEqual(expected, actual);
        }

    }
}
