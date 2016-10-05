using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.Logging;
using NHibernate;

namespace ShoppingCart.DAL.NHibernate
{
    public class ProductRepository : IProductRepository
    {
        private const int DefaultFirstResult = 0;
        private const int DefaultMaxResult = 50;
        private const string DefaultSortby = "id";
        private const int MaxNameLength = 50;
        private const string DefaultSortDirection = "asc";
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
                        if (entity.Name.Length > MaxNameLength) throw new Exception(
                            $"Name consists of more than {MaxNameLength} letters or null");
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

        public IList<Product> List(string filter, string sortby, string sortDirection, int firstResult, int maxResults)
        {
            if (firstResult < 0) firstResult = DefaultFirstResult;
            if ((maxResults <= 0) || (maxResults > 250)) maxResults = DefaultMaxResult;
            sortby = sortby ?? DefaultSortby;
            sortby = sortby.ToLowerInvariant();
            sortby = OrderByFuncs.ContainsKey(sortby) ? sortby : DefaultSortby;
            sortDirection = sortDirection ?? DefaultSortDirection;
            sortDirection = sortDirection.ToLowerInvariant();
            sortDirection = sortDirection.Contains("desc") ? "desc" : DefaultSortDirection;
            using (var session = NhibernateHelper.OpenSession())
            {
                try
                {
                    var query = session.QueryOver<Product>();
                    query.Skip(firstResult).Take(maxResults);

                    if (!string.IsNullOrEmpty(filter))
                    {
                        query
                            .WhereRestrictionOn(x => x.Name)
                            .IsLike($"%{filter}%");
                    }
                    var orderBy = query.OrderBy(OrderByFuncs[sortby]);
                    var products = (sortDirection != DefaultSortDirection) ? orderBy.Desc.List() : orderBy.Asc.List();
                    return products;
                }
                catch (Exception e)
                {
                    Log.Error("Exception occured when system tried to get the list of products from database", e);
                    throw;
                }
            }
        }

        public int Count(string filter)
        {
            using (var session = NhibernateHelper.OpenSession())
            {
                try
                {
                    var query = session.QueryOver<Product>();
                    if (!string.IsNullOrEmpty(filter))
                    {
                        query
                            .WhereRestrictionOn(x => x.Name)
                            .IsLike($"%{filter}%");
                    }
                    return query.List().Count;
                }
                catch (HibernateException e)
                {
                    Log.Error("Exception occured when system tried to get the count of products from database", e);
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
                catch (Exception e)
                {
                    Log.Error("Exception occured when system tried to get the list of products by id from database", e);
                    throw;
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
