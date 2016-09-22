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

        public IList<Product> List(string filter, string sortby, int? pageSize, int page)
        {
            sortby = sortby ?? DefaultSortby;
            sortby = sortby.ToLowerInvariant();
            sortby = OrderByFuncs.ContainsKey(sortby) ? sortby : DefaultSortby;
            using (var session = NhibernateHelper.OpenSession())
            {
                try
                {
                    var query = session.QueryOver<Product>();
                    if (pageSize.HasValue)
                    {
                        query.Take(pageSize.Value).Skip((int)(page * pageSize));
                    }
                    if (!string.IsNullOrEmpty(filter))
                    {
                        query
                            .WhereRestrictionOn(x => x.Name)
                            .IsLike($"{filter}%");
                    }

                    var products = query.OrderBy(OrderByFuncs[sortby]).Asc.List();
                    return products;
                }
                catch (Exception)
                {
                    Log.Error("Exception occured when system tried to get the list of products from database");
                    return null;
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