using NHibernate;
using ShoppingCart.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}