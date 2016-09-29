using System;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace ShoppingCart.DAL.NHibernate.Tests
{
    [TestFixture]
    public class ProductRepositoryTest
    {
        private ISessionFactory _sessionFactory;
        private Configuration _cfg;

        private readonly Product[] _list =
        {
            new Product {Name = "Car", Quantity = 5,Price = 15000},
            new Product {Name = "Car", Quantity = 7, Price = 20000},
            new Product {Name = "Apple", Quantity = 5, Price = 40},
            new Product {Name = "Apple", Quantity = 5, Price = 37},
            new Product {Name = "Apple", Quantity = 25, Price = 40}

        };

        private void CreateInitialData()
        {
            using (var session = NhibernateHelper.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                foreach (var product in _list)
                    session.Save(product);
                transaction.Commit();
            }
        }

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            _cfg = new Configuration();
            _cfg.Configure();
            _cfg.AddAssembly(typeof(Product).Assembly);
            _sessionFactory = _cfg.BuildSessionFactory();
        }

        [SetUp]
        public void SetupContext()
        {
            new SchemaExport(_cfg).Execute(false, true, false);
            CreateInitialData();
        }

        [Test]
        public void Can_create_new_product()
        {
            var product = new Product { Name = "Car", Quantity = 4, Price = 20000 };
            var repository = new ProductRepository();
            repository.Create(product);
            using (var session = _sessionFactory.OpenSession())
            {
                var fromDb = session.Get<Product>(product.Id);
                Assert.IsNotNull(fromDb);
                Assert.AreNotSame(product, fromDb);
                Assert.AreEqual(product.Name, fromDb.Name);
                Assert.AreEqual(product.Price, fromDb.Price);
                Assert.AreEqual(product.Quantity, fromDb.Quantity);
            }
        }

        [Test]
        public void Not_can_create_new_product_with_longname()
        {
            const int maxNameLength = 50;
            var product = new Product { Name = "0123456789 0123456789 0123456789 0123456789 0123456789", Quantity = 4, Price = 20 };
            var repository = new ProductRepository();
            Assert.Throws<Exception>(() => repository.Create(product),
                $"Name consists of more than {maxNameLength} letters or null");
        }

        [Test]
        public void Can_get_product_by_id()
        {
            var repository = new ProductRepository();
            var fromDb = repository.Get(_list[1].Id);
            Assert.IsNotNull(fromDb);
            Assert.AreNotSame(_list[1], fromDb);
            Assert.AreEqual(_list[1].Name, fromDb.Name);
        }

        [Test]
        public void Can_remove_product()
        {
            var product = _list[0];
            var repository = new ProductRepository();
            repository.Delete(product.Id);
            using (var session = _sessionFactory.OpenSession())
            {
                var fromDb = session.Get<Product>(product.Id);
                Assert.IsNull(fromDb);
            }
        }

        [Test]
        public void Can_get_count()
        {
            var repository = new ProductRepository();
            var countWithFilter = repository.Count("Apple");
            var countWithoutFilter = repository.Count(null);
            Assert.AreEqual(3, countWithFilter);
            Assert.AreEqual(5, countWithoutFilter);
        }

        [Test]
        public void Can_get_list()
        {
            var repository = new ProductRepository();
            var list1 = repository.List("Car", "price", "desc", 0, 250).Count;
            using (var session = _sessionFactory.OpenSession())
            {
                var fromDb = session.QueryOver<Product>().Where(x => x.Name == "Car").List().Count;
                Assert.AreEqual(fromDb, list1);
            }
        }
    }
}

