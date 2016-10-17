using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NHibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spring.Testing.Microsoft;


namespace ShoppingCart.DAL.NHibernate.Tests
{
    [TestClass]
    public class ProductRepositoryTest : AbstractTransactionalDbProviderSpringContextTests
    {
        public  ISessionFactory Sessionfactory { get; set; }
        public  ProductRepository ProductRepository { get; set; }

        private IList<Product> CreateInitialData(IEnumerable<Product> products)
        {
            var initialList = products;
            var enumerable = initialList as Product[] ?? initialList.ToArray();
            using (var session = Sessionfactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                foreach (var product in enumerable)
                    session.Save(product);
                transaction.Commit();
            }
            return enumerable.ToList();
        }

        [TestInitialize]
        public void SetupContext()
        {
            AdoTemplate.ExecuteNonQuery(CommandType.Text, "TRUNCATE TABLE product ");
        }

        [TestMethod]
        public void Can_create_new_product()
        {
            var expected = CreateProduct("Car");
            var repository = ProductRepository;

            repository.Create(expected);
            Sessionfactory.GetCurrentSession().Flush();

            using (var session = Sessionfactory.OpenSession())
            {
                var actual = session.Get<Product>(expected.Id);

                Assert.IsNotNull(actual);
                Assert.AreNotSame(expected, actual);
                CompareProducts(expected, actual);
            }
        }

        [TestMethod]
        public void Can_create_new_product_with_longname()
        {
            var repository = ProductRepository;
            AssertException.Throws<RepositoryException>(() => repository.Create(CreateProduct(GenerateName(50))));
            AssertException.Throws<RepositoryException>(() => repository.Create(CreateProduct(GenerateName(51))));
            AssertException.Throws<RepositoryException>(() => repository.Create(CreateProduct(GenerateName(52))));
        }

        [TestMethod]
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
            var repository = ProductRepository;
            var notExistId = expectedList.Last().Id + 1000;

            foreach (var expected in expectedList)
            {
                var actual = repository.Get(expected.Id);
                Assert.IsNotNull(actual);
                CompareProducts(expected, actual);
            }
            Assert.IsNull(repository.Get(notExistId));
        }

        [TestMethod]
        public void Can_remove_product_by_id()
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
            var product = expectedList.Last();
            var repository = ProductRepository;

            repository.Delete(product.Id);
            var notExistId = product.Id + 1000;
            Sessionfactory.GetCurrentSession().Flush();

            using (var session = Sessionfactory.OpenSession())
            {
                var anObject = session.Get<Product>(product.Id);
                Assert.IsNull(anObject);
                repository.Delete(notExistId);
            }
        }

        [TestMethod]
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

        [TestMethod]
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
            var repository = ProductRepository;

            var actual = repository.Count();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
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
            var repository = ProductRepository;

            var actual = repository.List();

            AssertList(expected, actual);
        }

        [TestMethod]
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
            var repository = ProductRepository;
            var expected = expectedList;

            var actual = repository.List("pp");

            AssertList(expected, actual);
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
            var repository = ProductRepository;
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

        private void CountAssert(int expected, string filter)
        {
            var repository = ProductRepository;

            var actual = repository.Count(filter);

            Assert.AreEqual(expected, actual);
        }

        private void AssertSortBy(IList<Product> expected, string sortBy)
        {
            var repository = ProductRepository;
            var actual = repository.List(null, sortBy);
            AssertList(expected, actual);
        }

        private void AssertSortDirection(IList<Product> expected, bool sortDirection)
        {
            var repository = ProductRepository;

            var actual = repository.List(null, null, sortDirection);

            AssertList(expected, actual);
        }

        private void AssertFirstResultAndMaxResult(IList<Product> expected, int firstResult, int maxResult)
        {
            var repository = ProductRepository;
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

        protected override string[] ConfigLocations => new[] { "assembly://ShoppingCart.DAL.NHibernate.Tests/ShoppingCart.DAL.NHibernate.Tests/DI.xml" };

    }

}
