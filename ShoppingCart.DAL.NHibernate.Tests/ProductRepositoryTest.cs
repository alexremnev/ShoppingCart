using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ShoppingCart.DAL.NHibernate.Tests
{
    [TestFixture]
    public class ProductRepositoryTest
    {
        private static IList<Product> CreateInitialData(IEnumerable<Product> products)
        {
            var initialList = products as IList<Product> ?? products.ToArray();
            using (var session = NhibernateHelper.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                foreach (var product in initialList)
                    session.Save(product);
                transaction.Commit();
            }
            return initialList;
        }

        [SetUp]
        public void SetupContext()
        {
            NhibernateHelper.Reset();
        }

        [Test]
        public void Can_create_new_product()
        {
            var expected = new Product { Name = "Car", Quantity = 4, Price = 20000 };
            var repository = new ProductRepository();
            repository.Create(expected);
            using (var session = NhibernateHelper.OpenSession())
            {
                var actual = session.Get<Product>(expected.Id);

                Assert.IsNotNull(actual);
                Assert.AreNotSame(expected, actual);
                Assert.AreEqual(expected.Id, actual.Id);
                Assert.AreEqual(expected.Name, actual.Name);
                Assert.AreEqual(expected.Price, actual.Price);
                Assert.AreEqual(expected.Quantity, actual.Quantity);
            }
        }

        [Test]
        public void Can_create_new_product_with_longname()
        {
            var repository = new ProductRepository();

            Assert.Throws<Exception>(
                () => repository.Create(new Product { Name = GenerateName(50), Quantity = 4, Price = 20 }));
            Assert.Throws<Exception>(
                () => repository.Create(new Product { Name = GenerateName(51), Quantity = 4, Price = 20 }));
            Assert.Throws<Exception>(
                () => repository.Create(new Product { Name = GenerateName(100), Quantity = 4, Price = 20 }));
        }

        [Test]
        public void Can_get_product_by_id()
        {
            Product[] list =
              {
            new Product {Name = "Car yellow", Quantity = 5, Price = 15000},
            new Product {Name = "car blue", Quantity = 7, Price = 20000},
            new Product {Name = "apple oneType", Quantity = 5, Price = 40},
            new Product {Name = "Apple anotherType", Quantity = 5, Price = 37},
            new Product {Name = "apple", Quantity = 25, Price = 40}
        };
            var expectedList = CreateInitialData(list);
            var count = expectedList.Count;
            var repository = new ProductRepository();
            var notExistId = expectedList[count - 1].Id + 1000;

            for (var i = 0; i < count - 1; i++)
            {
                var expected = expectedList[i];
                var actual = repository.Get(expected.Id);
                Assert.IsNotNull(actual);
                Assert.AreNotSame(expected, actual);
                Assert.AreEqual(expected.Name, actual.Name);
                Assert.AreEqual(expected.Quantity, actual.Quantity);
                Assert.AreEqual(expected.Price, actual.Price);
            }
            Assert.IsNull(repository.Get(notExistId));
        }

        [Test]
        public void Can_remove_product_by_id()
        {
            Product[] list =
              {
            new Product {Name = "Car yellow", Quantity = 5, Price = 15000},
            new Product {Name = "car blue", Quantity = 7, Price = 20000},
            new Product {Name = "apple oneType", Quantity = 5, Price = 40},
            new Product {Name = "Apple anotherType", Quantity = 5, Price = 37},
            new Product {Name = "apple", Quantity = 25, Price = 40}
        };
            var expectedList = CreateInitialData(list);
            var product = expectedList.Last();
            var repository = new ProductRepository();
            repository.Delete(product.Id);
            var notExistId = product.Id + 1000;
            using (var session = NhibernateHelper.OpenSession())
            {
                var anObject = session.Get<Product>(product.Id);
                Assert.IsNull(anObject);
                Assert.Throws<ArgumentNullException>(() => repository.Delete(notExistId));
            }
        }

        [Test]
        public void Can_get_count_with_filter()
        {
            var noise = new List<Product>{
                new Product { Name = "Car yellow", Quantity = 5, Price = 15000 },
            new Product { Name = "car blue", Quantity = 7, Price = 20000 },
        };
            var expectedList = new List<Product>{
            new Product { Name = "apple oneType", Quantity = 5, Price = 40 },
            new Product { Name = "Apple anotherType", Quantity = 5, Price = 37 },
            new Product { Name = "apple", Quantity = 25, Price = 40 }

        };
            var all = expectedList.Union(noise);
            CreateInitialData(all);
            var expected = expectedList.Count;

            AssertCount(expected, "Apple");
            AssertCount(expected, "apple");
            AssertCount(expected, "APPLE");
            AssertCount(expected, "Pp");
            AssertCount(expected, "pP");
        }

        [Test]
        public void Can_get_count_without_filter()
        {
            Product[] list =
              {
            new Product {Name = "Car yellow", Quantity = 5, Price = 15000},
            new Product {Name = "car blue", Quantity = 7, Price = 20000},
            new Product {Name = "apple oneType", Quantity = 5, Price = 40},
            new Product {Name = "Apple anotherType", Quantity = 5, Price = 37},
            new Product {Name = "apple", Quantity = 25, Price = 40}
        };
            var expected = CreateInitialData(list).Count;
            var repository = new ProductRepository();

            var actual = repository.Count(null);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Can_get_list_without_filters()
        {
            Product[] list =
              {
            new Product {Name = "Car yellow", Quantity = 5, Price = 15000},
            new Product {Name = "car blue", Quantity = 7, Price = 20000},
            new Product {Name = "apple oneType", Quantity = 5, Price = 40},
            new Product {Name = "Apple anotherType", Quantity = 5, Price = 37},
            new Product {Name = "apple", Quantity = 25, Price = 40}
        };
            var expected = CreateInitialData(list);
            var repository = new ProductRepository();

            var actual = repository.List(null, null, null, 0, 0);

            AssertList(expected, actual);
        }

        [Test]
        public void Can_get_list_with_filter_filter()
        {
            var expectedList = new List<Product>{
                new Product { Name = "Car yellow", Quantity = 5, Price = 15000 },
            new Product { Name = "car blue", Quantity = 7, Price = 20000 },
        };
            var noise = new List<Product>{
            new Product { Name = "apple oneType", Quantity = 5, Price = 40 },
            new Product { Name = "Apple anotherType", Quantity = 5, Price = 37 },
            new Product { Name = "apple", Quantity = 25, Price = 40 }

        };
            var all = expectedList.Union(noise);
            CreateInitialData(all);
            var repository = new ProductRepository();
            var expected = expectedList;

            var actual = repository.List("Car", null, null, 0, 0);

            AssertList(expected, actual);
        }

        [Test]
        public void Can_get_list_with_not_full_math_filter()
        {
            var noise = new List<Product>{
                new Product { Name = "Car yellow", Quantity = 5, Price = 15000 },
            new Product { Name = "car blue", Quantity = 7, Price = 20000 },
        };
            var expectedList = new List<Product>{
            new Product { Name = "apple oneType", Quantity = 5, Price = 40 },
            new Product { Name = "Apple anotherType", Quantity = 5, Price = 37 },
            new Product { Name = "apple", Quantity = 25, Price = 40 }

        };
            var all = noise.Union(expectedList);
            CreateInitialData(all);
            var repository = new ProductRepository();
            var expected = expectedList;

            var actual = repository.List("pp", null, "null", 0, 0);

            AssertList(expected, actual);
        }

        [Test]
        public void Can_get_list_with_filter_sortBy()
        {
            Product[] list =
              {
            new Product {Name = "Car yellow", Quantity = 5, Price = 15000},
            new Product {Name = "car blue", Quantity = 7, Price = 20000},
            new Product {Name = "apple oneType", Quantity = 5, Price = 40},
            new Product {Name = "Apple anotherType", Quantity = 5, Price = 37},
            new Product {Name = "apple", Quantity = 25, Price = 40}
        };
            var expectedList = CreateInitialData(list).ToList();

            var expectedResultByPrice = expectedList.OrderBy(x => x.Price).ToList();
            var expectedResultByQuantity = expectedList.OrderBy(x => x.Quantity).ToList();

            AssertSortBy(expectedList, "Id");
            AssertSortBy(expectedResultByPrice, "price");
            AssertSortBy(expectedResultByQuantity, "Quantity");
        }

        [Test]
        public void Can_get_list_with_filter_sortDirection()
        {
            Product[] list =
              {
            new Product {Name = "Car yellow", Quantity = 5, Price = 15000},
            new Product {Name = "car blue", Quantity = 7, Price = 20000},
            new Product {Name = "apple oneType", Quantity = 5, Price = 40},
            new Product {Name = "Apple anotherType", Quantity = 5, Price = 37},
            new Product {Name = "apple", Quantity = 25, Price = 40}
        };
            var expectedAsc = CreateInitialData(list);
            var expectedDesc = expectedAsc.Reverse().ToList();

            AssertSortDirection(expectedDesc, "desc");
            AssertSortDirection(expectedDesc, "DESC");
            AssertSortDirection(expectedAsc, "asc");
            AssertSortDirection(expectedAsc, "ASC");
            AssertSortDirection(expectedAsc, "ddd");
        }

        [Test]
        public void Can_get_list_with_filter_firstResult_and_maxResult()
        {
            Product[] list =
              {
            new Product {Name = "Car yellow", Quantity = 5, Price = 15000},
            new Product {Name = "car blue", Quantity = 7, Price = 20000},
            new Product {Name = "apple oneType", Quantity = 5, Price = 40},
            new Product {Name = "Apple anotherType", Quantity = 5, Price = 37},
            new Product {Name = "apple", Quantity = 25, Price = 40}
        };
            var repository = new ProductRepository();
            var expected = CreateInitialData(list);

            AssertFirstResultAndMaxResult(expected, 0, 2);
            AssertFirstResultAndMaxResult(expected, 0, 6);
            AssertFirstResultAndMaxResult(expected, 1, 3);
            AssertFirstResultAndMaxResult(expected, 6, 3);
            AssertFirstResultAndMaxResult(expected, -1, 3);
            AssertList(expected, repository.List(null, null, null, 0, -3));
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

        private static void AssertList(IList<Product> expectedList, IList<Product> actualList)
        {
            Assert.AreEqual(expectedList.Count, actualList.Count);
            for (var i = 0; i < actualList.Count; i++)
            {
                var actual = actualList[i];
                var expected = expectedList[i];
                Assert.AreEqual(expected.Id, actual.Id);
                Assert.AreEqual(expected.Name, actual.Name);
                Assert.AreEqual(expected.Quantity, actual.Quantity);
                Assert.AreEqual(expected.Price, actual.Price);
            }
        }
        private static void AssertCount(int expected, string filter)
        {
            var repository = new ProductRepository();

            var actual = repository.Count(filter);

            Assert.AreEqual(expected, actual);
        }

        private static void AssertSortBy(IList<Product> expected, string sortBy)
        {
            var repository = new ProductRepository();
            var actual = repository.List(null, sortBy, "null", 0, 0);
            AssertList(expected, actual);
        }

        private static void AssertSortDirection(IList<Product> expected, string sortDirection)
        {
            var repository = new ProductRepository();

            var actual = repository.List(null, null, sortDirection, 0, 0);

            AssertList(expected, actual);
        }
        private static void AssertFirstResultAndMaxResult(IList<Product> expected, int firstResult, int maxResult)
        {
            var repository = new ProductRepository();
            expected = expected.Skip(firstResult).Take(maxResult).ToList();

            var actual = repository.List(null, null, null, firstResult, maxResult);

            AssertList(expected, actual);
        }
    }
}
