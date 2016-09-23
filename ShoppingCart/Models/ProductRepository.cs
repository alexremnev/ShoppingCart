using System;
using ShoppingCart.Models.Domain;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.Logging;
using NHibernate;

namespace ShoppingCart.Models
{
    public class ProductRepository : IProductRepository
    {
        private const string DefaultSortby = "id";
        private const int DefaultMaxResult = 50;
        private const int DefaultFirstResult = 0;
        private static readonly ILog Log = LogManager.GetLogger<ProductRepository>();
        private static readonly IDictionary<string, Expression<Func<Product, object>>> OrderByFuncs = new Dictionary<string, Expression<Func<Product, object>>>
        {
            {DefaultSortby, p => p.Id },
            {"price", p => p.Price },
            {"quantity", p => p.Quantity }
        };
        public void Create(Product entity)
        {
            using (var session = NhibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        if (entity.Name.Length > 50) throw new Exception("Name consists of more than 50 letters or null");
                        session.Save(entity);
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (HibernateException exception)
                        {
                            Log.Error("Exception occurred when system tried to roll back transaction", exception);
                        }
                        Log.Error(e.Message, e);
                        throw;
                    }
                }
            }
        }

        public IList<Product> List(string filter, string sortby, int? maxResult, int? firstResult)
        {
            sortby = sortby ?? DefaultSortby;
            sortby = sortby.ToLowerInvariant();
            maxResult = maxResult ?? DefaultMaxResult;
            maxResult = maxResult > 250 ? DefaultMaxResult : maxResult;
            firstResult = firstResult ?? DefaultFirstResult;
            var isSortByDescending = sortby.Contains("desc");
            var isContainOrderByFuncsKey = false;

            foreach (var key in OrderByFuncs)
            {
                if (!sortby.Contains(key.Key)) continue;
                sortby = key.Key;
                isContainOrderByFuncsKey = true;
                break;
            }

            if (!isContainOrderByFuncsKey) sortby = DefaultSortby;

            using (var session = NhibernateHelper.OpenSession())
            {
                try
                {
                    var query = session.QueryOver<Product>();
                    firstResult = firstResult.Value * maxResult;
                    query.Skip(firstResult.Value).Take(maxResult.Value);

                    if (!string.IsNullOrEmpty(filter))
                    {
                        query
                            .WhereRestrictionOn(x => x.Name)
                            .IsLike($"{filter}%");
                    }
                    query = isSortByDescending ? query.OrderBy(OrderByFuncs[sortby]).Desc : query.OrderBy(OrderByFuncs[sortby]).Asc;
                    var products = query.List();
                    return products;
                }
                catch (Exception e)
                {
                    Log.Error("Exception occured when system tried to get the list of products from database", e);
                    throw;
                }
            }
        }

        public Product Get(int id)
        {
            using (var session = NhibernateHelper.OpenSession())
            {
                try
                {
                    return session.QueryOver<Product>().Where(x => x.Id == id).SingleOrDefault();
                }
                catch (Exception)
                {
                    Log.Error("Exception occured when system tried to get the list of products by id from database");
                    return null;
                }
            }
        }

        public void Delete(int id)
        {
            using (var session = NhibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var product = Get(id);
                        session.Delete(product);
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (HibernateException exception)
                        {
                            Log.Error("Exception occurred when system tried to roll back transaction", exception);
                        }
                        Log.Error("Exception occured when system tried to delete the object by id", e);
                        throw;
                    }
                }
            }
        }
    }
}