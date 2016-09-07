using NHibernate;
using ShoppingCart.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using NHibernate.Hql.Ast.ANTLR;

namespace ShoppingCart.Models
{
    public class ProductsRepository
    {
        public void Add(Products newProducts)
        {
            using (ISession session = NhibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(newProducts);
                    transaction.Commit();
                }
            }
        }

        public Products FindById(int id)
        {
            using (ISession session = NhibernateHelper.OpenSession())
            {

                var result = session.QueryOver<Products>().Where(x => x.Id == id).SingleOrDefault();
                return result ?? new Products();

            }
        }

        public void Delete(Products product)
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
        public List<Products> ShowAllProducts()
        {
            using (ISession session = NhibernateHelper.OpenSession())
            {

                var result = session.QueryOver<Products>().Where(x=>x.Id<100).List();
                return (List<Products>) result;

            }
        }

    }
}