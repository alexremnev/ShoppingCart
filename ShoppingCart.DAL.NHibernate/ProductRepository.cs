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
        private static readonly ILog Log = LogManager.GetLogger<ProductRepository>();
        private static readonly IDictionary<string, Expression<Func<Product, object>>> OrderByFuncs = new Dictionary
            <string, Expression<Func<Product, object>>>
            {
                {DefaultSortby, p => p.Id},
                {"price", p => p.Price},
                {"quantity", p => p.Quantity}
            };

        public void Create(Product entity)
        {
            if (entity == null) throw new RepositoryException("Name is null");
            if (entity.Name.Length > MaxNameLength)
                throw new RepositoryException($"Name consists of more than {MaxNameLength} letters");
            using (var session = NhibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.Save(entity);
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Exception occured when system tried save the product ={entity}", e);
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

        public IList<Product> List(string filter = null, string sortby = null, bool sortDirection = true, int firstResult = 0, int maxResults = 50)
        {
            if (firstResult < 0) firstResult = DefaultFirstResult;
            if (maxResults < 0) maxResults = DefaultMaxResult;
            sortby = sortby ?? DefaultSortby;
            sortby = sortby.ToLowerInvariant();
            sortby = OrderByFuncs.ContainsKey(sortby) ? sortby : DefaultSortby;

            using (var session = NhibernateHelper.OpenSession())
            {
                try
                {
                    var query = session.QueryOver<Product>();
                    query.Skip(firstResult).Take(maxResults);

                    GetListWithFilter(filter, query);
                    var orderBy = query.OrderBy(OrderByFuncs[sortby]);
                    var products = (sortDirection) ? orderBy.Asc : orderBy.Desc;
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
            using (var session = NhibernateHelper.OpenSession())
            {
                try
                {
                    var query = session.QueryOver<Product>();
                    GetListWithFilter(filter, query);
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

        public Product Get(int id)
        {
            using (var session = NhibernateHelper.OpenSession())
            {
                try
                {
                    return session.Get<Product>(id);
                }
                catch (Exception e)
                {
                    Log.Error(
                        $"Exception occured when system tried to get the list of products by id ={id} from database", e);
                    throw;
                }
            }
        }

        public void Delete(int id)
        {
            var product = Get(id);
            try
            {
                if (product == null) throw new RepositoryException();
            }
            catch (RepositoryException)
            {
                Log.Error("Exception occured when you tried delete product which equal null");
                throw;
            }

            using (var session = NhibernateHelper.OpenSession())
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
        private static void GetListWithFilter(string filter, IQueryOver<Product, Product> query)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                query
                    .WhereRestrictionOn(x => x.Name)
                    .IsInsensitiveLike($"%{filter}%");
            }
        }
    }
}
