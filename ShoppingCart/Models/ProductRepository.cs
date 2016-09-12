using NHibernate;
using ShoppingCart.Models.Domain;
using System.Collections.Generic;


namespace ShoppingCart.Models
{
    public class ProductRepository: IProductRepository
    {
        public void Add(Product newProducts)
        {
            using (var session = NhibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(newProducts);
                    transaction.Commit();
                }
            }
        }

        public Product FindById(int id)
        {
            using (ISession session = NhibernateHelper.OpenSession())
            {

                var result = session.QueryOver<Product>().Where(x => x.Id == id).SingleOrDefault();
                return result ?? new Product();

            }
        }

        public void Delete(Product product)
        {
            using (ISession session = NhibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(product);
                    transaction.Commit();
                }
            }
                
            
        }
        public List<Product> ShowAllProducts()
        {
            using (ISession session = NhibernateHelper.OpenSession())
            {

                var result = session.QueryOver<Product>().Where(x=>x.Id<100).List();
                return (List<Product>) result;

            }
        }

    }
}