using System;
using ShoppingCart.Models.Domain;
using System.Collections.Generic;

namespace ShoppingCart.Models
{
    public class ProductRepository : IProductRepository
    {
        public void Create(Product newProducts)
        {
            using (var session = NhibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.Save(newProducts);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        public IList<Product> Read()
        {
            using (var session = NhibernateHelper.OpenSession())
            {
                return session.QueryOver<Product>().List();
            }
        }

        public Product FindById(int id)
        {
            using (var session = NhibernateHelper.OpenSession())
            {
                var result = session.QueryOver<Product>().Where(x => x.Id == id).SingleOrDefault();
                return result ?? new Product();
            }
        }

        public void Delete(Product product)
        {
            using (var session = NhibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.Delete(product);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }
    }

}