using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NHibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Bytecode.Lightweight;
using Spring.Data.NHibernate;
using Spring.Testing.Microsoft;

namespace ShoppingCart.DAL.NHibernate.Tests
{
    [TestClass]
    public class ProductRepositoryTest : AbstractTransactionalSpringContextTests
    {
        public ISessionFactory Sessionfactory { get; set; }
        public ProductRepository ProductRepository { get; set; }
        private static readonly ILog Log = LogManager.GetLogger<ProductRepositoryTest>();

        private void CreateInitialData(IList<Product> products)
        {
            var initialList = products;

            using (var session = Sessionfactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                foreach (var product in initialList)
                    session.Save(product);
                transaction.Commit();
            }
        }








        //[TestMethod]
        //public void Test()
        //{
        //    var springlocalSessionFactoryObject = new SpringLocalSessionFactoryObject();
        //    springlocalSessionFactoryObject.ByteCodeProvider = new SpringByteCodeProvider();

        //    var nHibernate = springlocalSessionFactoryObject.NHibernate;

        //    var userTypeInstance = nHibernate.ByteCodeProvider.Factory.CreateInstance("EncryptedString");
        //}

        //class SpringLocalSessionFactoryObject
        //{
        //    public SpringLocalSessionFactoryObject()
        //    {
        //        NHibernate = new NHibernate();
        //    }

        //    public IByteCodeProvider ByteCodeProvider
        //    {
        //        set
        //        {
        //            NHibernate.ByteCodeProvider = value;
        //        }
        //    }

        //    public NHibernate NHibernate { get; }
        //}

        //class NHibernate
        //{
        //    private IByteCodeProvider _byteCodeProvider;

        //    public IByteCodeProvider ByteCodeProvider
        //    {
        //        get { return _byteCodeProvider ?? (_byteCodeProvider = new DefaultNHibernateByteCodeProvider()); }
        //        set { _byteCodeProvider = value; }
        //    }
        //}

        //interface IFactory
        //{
        //    object CreateInstance(string type);
        //}

        //private class SpringFactory : IFactory
        //{
        //    public object CreateInstance(string type)
        //    {
        //        //                return springContext.GetObject(type);
        //        return type;
        //    }
        //}

        //private class DefaultNHibernateFactory : IFactory
        //{
        //    public object CreateInstance(string type)
        //    {
        //        return new EncryptedString();
        //    }
        //}

        //interface IByteCodeProvider
        //{
        //    IFactory Factory { get; }
        //}

        //private class SpringByteCodeProvider : IByteCodeProvider
        //{
        //    public SpringByteCodeProvider()
        //    {
        //        Factory = new SpringFactory();
        //    }

        //    public IFactory Factory { get; }
        //}

        //private class DefaultNHibernateByteCodeProvider : IByteCodeProvider
        //{
        //    public IFactory Factory
        //    {
        //        get { return new DefaultNHibernateFactory(); }
        //    }
        //}































        [TestInitialize]
        public void SetupContext()
        {
            Sessionfactory.OpenSession().CreateQuery("DELETE FROM Product ").ExecuteUpdate();
        }

        [TestMethod]
        public void Can_create_new_product()
        {
            var expected = CreateProduct("Car");
            var repository = ProductRepository;

            repository.Create(expected);
            Sessionfactory.GetCurrentSession().Transaction.Commit();
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void Can_create_product_with_null_entity()
        {
            var repository = ProductRepository;
            repository.Create(null);
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
            CreateInitialData(list);
            var repository = ProductRepository;
            var notExistId = list.Last().Id + 1000;

            foreach (var expected in list)
            {
                var actual = repository.Get(expected.Id);
                Assert.IsNotNull(actual);
                CompareProducts(expected, actual);
            }
            Assert.IsNull(repository.Get(notExistId));
        }

        [TestMethod]
        [ExpectedException(typeof(RepositoryException))]
        public void Can_get_product_by_negative_id()
        {
            const int id = -1;
            var repository = ProductRepository;

            repository.Get(id);
        }

        [TestMethod]
        public void Can_update_product()
        {
            var product = new Product { Name = "Car yellow", Quantity = 5, Price = 15000 };
            CreateInitialData(new List<Product> { product });
            var expected = new Product { Id = product.Id, Name = "Car blue", Quantity = 3, Price = 20000 };
            var repository = ProductRepository;

            repository.Update(expected);

            Sessionfactory.GetCurrentSession().Transaction.Commit();
            Sessionfactory.GetCurrentSession().Flush();
            using (var session = Sessionfactory.OpenSession())
            {
                var actual = session.Get<Product>(product.Id);
                Assert.AreEqual(expected.Name, actual.Name);
                Assert.AreEqual(expected.Quantity, actual.Quantity);
                Assert.AreEqual(expected.Price, actual.Price);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Can_update_product_with_null_entity()
        {

            var repository = ProductRepository;

            repository.Update(null);
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
            CreateInitialData(list);
            var product = list.Last();
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
            CreateInitialData(all.ToList());
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
            CreateInitialData(list);
            var expected = list.Length;
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
            CreateInitialData(list);
            var expected = list;
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
            CreateInitialData(all.ToList());
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
            CreateInitialData(list);

            var expectedResultByPrice = list.OrderBy(x => x.Price).ToList();
            var expectedResultByQuantity = list.OrderBy(x => x.Quantity).ToList();

            AssertSortBy(list, "Id");
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
            CreateInitialData(list);
            var expectedAsc = list;
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
            CreateInitialData(list);
            var expected = list;

            AssertFirstResultAndMaxResult(expected, 0, 2);
            AssertFirstResultAndMaxResult(expected, 0, 6);
            AssertFirstResultAndMaxResult(expected, 1, 3);
            AssertFirstResultAndMaxResult(expected, 6, 3);
            AssertFirstResultAndMaxResult(expected, -1, 3);
            AssertList(expected, repository.List(null, null, true, 0, -3));
        }

        [TestMethod]
        public void Can_get_product_by_name()
        {
            const string name = "Car";
            var product = new Product { Name = "Car" };
            var list = new List<Product> { product };
            CreateInitialData(list);
            var expected = product;
            var repository = ProductRepository;
            var actual = repository.GetByName(name);

            CompareProducts(expected, actual);
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

        protected override string[] ConfigLocations => new[]
        {
            "config://spring/objects",
            "assembly://ShoppingCart.DAL.NHibernate/ShoppingCart.DAL.NHibernate/config.xml"
        };
    }
}

