﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.Logging;
using NHibernate;
using Spring.Transaction.Interceptor;

namespace ShoppingCart.DAL.NHibernate
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private const int DefaultFirstResult = 0;
        private const string DefaultSortby = "id";
        private static readonly ILog Log = LogManager.GetLogger<ProductRepository>();

        private static readonly IDictionary<string, Expression<Func<Product, object>>> OrderByFuncs = new Dictionary
            <string, Expression<Func<Product, object>>>
            {
                {DefaultSortby, p => p.Id},
                {"price", p => p.Price},
                {"quantity", p => p.Quantity}
            };

        public IList<Product> List(string filter = null, string sortby = null, bool isAscending = true,
             int firstResult = 0, int maxResults = 50)
        {

            if (firstResult < 0) firstResult = DefaultFirstResult;
            if (maxResults < 0) maxResults = Count(filter);
            if (maxResults == 0) return new List<Product>();
            sortby = sortby ?? DefaultSortby;
            sortby = sortby.ToLowerInvariant();
            sortby = OrderByFuncs.ContainsKey(sortby) ? sortby : DefaultSortby;

            using (var session = Sessionfactory.OpenSession())
            {
                try
                {
                    var query = session.QueryOver<Product>();
                    query.Skip(firstResult).Take(maxResults);

                    SetUpFilter(filter, query);
                    var orderBy = query.OrderBy(OrderByFuncs[sortby]);
                    var products = (isAscending) ? orderBy.Asc : orderBy.Desc;
                    return products.List();
                }
                catch (Exception e)
                {
                    Log.Error("Exception occured when system tried to get the list of products from database", e);
                    throw;
                }
            }
        }

        public int Count(string filter = null)
        {
            using (var session = Sessionfactory.OpenSession())
            {
                try
                {
                    var query = session.QueryOver<Product>();
                    SetUpFilter(filter, query);
                    return query.RowCount();
                }
                catch (HibernateException e)
                {
                    Log.Error(
                        $"Exception occured when system tried to get the count of products from database with filter ={filter}",
                        e);
                    throw;
                }
            }
        }

        protected override ILog GetLog()
        {
            return Log;
        }

        protected override string GetNameOfEntity()
        {
            return "product";
        }

        [Transaction]
        public void Delete(int id)
        {
            var product = Get(id);

            if (product == null) return;

            using (var session = Sessionfactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.Delete(product);
                        transaction.Commit();

                    }
                    catch (Exception e)
                    {
                        Log.Error($"Exception occured when system tried to delete the object by id= {id}", e);
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (HibernateException exception)
                        {
                            Log.Error("Exception occurred when system tried to roll back transaction", exception);
                        }
                        throw;
                    }
                }
            }
        }

        private static void SetUpFilter(string filter, IQueryOver<Product, Product> query)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                query
                    .WhereRestrictionOn(x => x.Name)
                    .IsInsensitiveLike($"%{filter}%");
            }
        }

        public Product GetByName(string name)
        {
            Product product;
            if (string.IsNullOrEmpty(name)) return null;
            using (var session = Sessionfactory.OpenSession())
            {
                try
                {
                    product = session.QueryOver<Product>().Where(m => m.Name == name).SingleOrDefault();
                }
                catch (Exception e)
                {
                    Log.Error(
                        "Exception occured when system tried to get the product by name", e);
                    throw;
                }
            }
            return product;
        }
    }
}

