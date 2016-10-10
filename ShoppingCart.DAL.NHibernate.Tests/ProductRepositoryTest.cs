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
            var initialList = products.ToList();
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
            var expected = CreateProduct("Car");
            var repository = new ProductRepository();
            repository.Create(expected);
            using (var session = NhibernateHelper.OpenSession())
            {
                var actual = session.Get<Product>(expected.Id);

                Assert.IsNotNull(actual);
                Assert.AreNotSame(expected, actual);
                CompareProducts(expected, actual);
            }
        }

        [Test]
        public void Can_create_new_product_with_longname()
        {
            var repository = new ProductRepository();

            Assert.Throws<RepositoryException>(
                () => repository.Create(CreateProduct(GenerateName(50))));
            Assert.Throws<RepositoryException>(
                () => repository.Create(CreateProduct(GenerateName(51))));
            Assert.Throws<RepositoryException>(
                () => repository.Create(CreateProduct(GenerateName(100))));
        }

        [Test]
        public void Can_get_product_by_id()
        {
            Product[] list =
              {
            CreateProduct("Car yellow",5,15000),
            CreateProduct("car blue",7,20000),
            CreateProduct("apple oneType",5,40),
            CreateProduct("Apple anotherType",5,37),
            CreateProduct("apple",25,40)

        };
            var expectedList = CreateInitialData(list);
            var repository = new ProductRepository();
            var notExistId = expectedList.Last().Id + 1000;

            foreach (var expected in expectedList)
            {
                var actual = repository.Get(expected.Id);
                Assert.IsNotNull(actual);
                CompareProducts(expected, actual);
            }
            Assert.IsNull(repository.Get(notExistId));
        }

        [Test]
        public void Can_remove_product_by_id()
        {
            // given
            Product[] list =
              {
            CreateProduct("Car yellow",5,15000),
            CreateProduct("car blue",7,20000),
            CreateProduct("apple oneType",5,40),
            CreateProduct("Apple anotherType",5,37),
            CreateProduct("apple",25,40)
        };
            var expectedList = CreateInitialData(list);
            var product = expectedList.Last();
            var repository = new ProductRepository();
            //when
            repository.Delete(product.Id);
            var notExistId = product.Id + 1000;
            //then
            using (var session = NhibernateHelper.OpenSession())
            {
                var anObject = session.Get<Product>(product.Id);
                Assert.IsNull(anObject);
                Assert.Throws<RepositoryException>(() => repository.Delete(notExistId));
            }
        }

        [Test]
        public void Can_get_count_with_filter()
        {
            var noise = new List<Product>{
                CreateProduct("Car yellow",5,15000),
                CreateProduct("car blue",7,20000)
        };
            var expectedList = new List<Product>{
            CreateProduct("apple oneType",5,40),
            CreateProduct("Apple anotherType",5,37),
            CreateProduct("apple",25,40)

        };
            var all = expectedList.Union(noise);
            CreateInitialData(all);
            var expected = expectedList.Count;

            CountAssert(expected, "Apple");
            CountAssert(expected, "apple");
            CountAssert(expected, "APPLE");
            CountAssert(expected, "Pp");
            CountAssert(expected, "pP");
        }

        [Test]
        public void Can_get_count_without_filter()
        {
            Product[] list =
              {
            CreateProduct("Car yellow",5,15000),
            CreateProduct("car blue",7,20000),
            CreateProduct("apple oneType",5,40),
            CreateProduct("Apple anotherType",5,37),
            CreateProduct("apple",25,40)
        };
            var expected = CreateInitialData(list).Count;
            var repository = new ProductRepository();

            var actual = repository.Count();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Can_get_list_without_filters()
        {
            Product[] list =
              {
            CreateProduct("Car yellow",5,15000),
            CreateProduct("car blue",7,20000),
            CreateProduct("apple oneType",5,40),
            CreateProduct("Apple anotherType",5,37),
            CreateProduct("apple",25,40)
        };
            var expected = CreateInitialData(list);
            var repository = new ProductRepository();

            var actual = repository.List();

            AssertList(expected, actual);
        }

        [Test]
        public void Can_get_list_with_filter()
        {
            var noise = new List<Product>{
                CreateProduct("Car yellow",5,15000),
                 CreateProduct("car blue",7,20000)
        };
            var expectedList = new List<Product>{
            CreateProduct("apple oneType",5,40),
            CreateProduct("Apple anotherType",5,37),
            CreateProduct("apple",25,40)

        };
            var all = noise.Union(expectedList);
            CreateInitialData(all);
            var repository = new ProductRepository();
            var expected = expectedList;

            var actual = repository.List("pp");

            AssertList(expected, actual);
        }

        [Test]
        public void Can_get_list_with_filter_sortBy()
        {
            Product[] list =
              {
            CreateProduct("Car yellow",5,15000),
            CreateProduct("car blue",7,20000),
            CreateProduct("apple oneType",5,40),
            CreateProduct("Apple anotherType",5,37),
            CreateProduct("apple",25,40)
        };
            var expectedList = CreateInitialData(list);

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
            CreateProduct("Car yellow",5,15000),
            CreateProduct("car blue",7,20000),
            CreateProduct("apple oneType",5,40),
            CreateProduct("Apple anotherType",5,37),
            CreateProduct("apple",25,40)
        };
            var expectedAsc = CreateInitialData(list);
            var expectedDesc = expectedAsc.Reverse().ToList();

            AssertSortDirection(expectedAsc, true);
            AssertSortDirection(expectedDesc, false);
        }

        [Test]
        public void Can_get_list_with_filter_firstResult_and_maxResult()
        {
            Product[] list =
              {
            CreateProduct("Car yellow",5,15000),
            CreateProduct("car blue",7,20000),
            CreateProduct("apple oneType",5,40),
            CreateProduct("Apple anotherType",5,37),
            CreateProduct("apple",25,40)
        };
            var repository = new ProductRepository();
            var expected = CreateInitialData(list);

            AssertFirstResultAndMaxResult(expected, 0, 2);
            AssertFirstResultAndMaxResult(expected, 0, 6);
            AssertFirstResultAndMaxResult(expected, 1, 3);
            AssertFirstResultAndMaxResult(expected, 6, 3);
            AssertFirstResultAndMaxResult(expected, -1, 3);
            AssertList(expected, repository.List(null, null, true, 0, -3));
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
                CompareProducts(expected, actual);
            }
        }

        private static void CountAssert(int expected, string filter)
        {
            var repository = new ProductRepository();

            var actual = repository.Count(filter);

            Assert.AreEqual(expected, actual);
        }

        private static void AssertSortBy(IList<Product> expected, string sortBy)
        {
            var repository = new ProductRepository();
            var actual = repository.List(null, sortBy);
            AssertList(expected, actual);
        }

        private static void AssertSortDirection(IList<Product> expected, bool sortDirection)
        {
            var repository = new ProductRepository();

            var actual = repository.List(null, null, sortDirection);

            AssertList(expected, actual);
        }

        private static void AssertFirstResultAndMaxResult(IList<Product> expected, int firstResult, int maxResult)
        {
            var repository = new ProductRepository();
            expected = expected.Skip(firstResult).Take(maxResult).ToList();

            var actual = repository.List(null, null, true, firstResult, maxResult);

            AssertList(expected, actual);
        }
        private static void CompareProducts(Product expected, Product actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Price, actual.Price);
            Assert.AreEqual(expected.Quantity, actual.Quantity);
        }

        private static Product CreateProduct(string name = "NewProduct", int quantity = 5, decimal price = 30)
        {
            return new Product { Name = name, Quantity = quantity, Price = price };
        }
    }
}
