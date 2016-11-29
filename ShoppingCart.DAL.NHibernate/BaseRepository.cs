using System;
using System.Collections.Generic;
using Common.Logging;
using NHibernate;
using Spring.Data.NHibernate;
using Spring.Transaction.Interceptor;

namespace ShoppingCart.DAL.NHibernate
{
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity, new()
    {
        private readonly ILog _log;
        private readonly string _entityName;

        protected BaseRepository(ILog log, string entityName)
        {
            _log = log;
            _entityName = entityName;
        }

        public ISessionFactory Sessionfactory { get; set; }
        private const int DefaultFirstResult = 0;

        [Transaction]
        public void Create(T entity)
        {
            try
            {
                if (entity == null) throw new ArgumentNullException(nameof(entity));
                var ht = new HibernateTemplate(Sessionfactory);
                ht.Save(entity);
            }
            catch (Exception e)
            {
                _log.Error($"Exception occured when system tried to create an {_entityName}", e);
                throw;
            }
        }

        public IList<T> List(int firstResult = 0, int maxResults = 50, Func<IQueryOver<T, T>, IQueryOver<T, T>> applyFilter = null)
        {
            if (firstResult < 0) firstResult = DefaultFirstResult;
            if (maxResults == 0) return new List<T>();
            try
            {
                using (var session = Sessionfactory.OpenSession())
                {
                    var query = session.QueryOver<T>();
                    var list = query.Skip(firstResult);
                    if (maxResults > 0) list.Take(maxResults);
                    applyFilter?.Invoke(query);
                    return list.List();
                }
            }
            catch (Exception e)
            {
                _log.Error($"Exception occured when you tried to get the list of {_entityName}s from database", e);
                throw;
            }
        }

        public T Get(int id)
        {
            using (var session = Sessionfactory.OpenSession())
            {
                try
                {
                    if (id <= 0) throw new RepositoryException("Id can not be below 1");
                    return session.Get<T>(id);
                }
                catch (Exception e)
                {
                    _log.Error(
                        $"Exception occured when system tried to get the list of customers by id ={id} from database", e);
                    throw;
                }
            }
        }

        [Transaction]
        public void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            try
            {
                var ht = new HibernateTemplate(Sessionfactory);
                ht.Update(entity);
            }
            catch (Exception e)
            {
                _log.Error($"Exception occured when system tried to update {_entityName}", e);
                throw;
            }
        }

        T IRepository<T>.NewEntity()
        {
            return NewEntity();
        }

        public int Count(Action<IQueryOver<T, T>> applyFilter = null)
        {
            using (var session = Sessionfactory.OpenSession())
            {
                try
                {
                    var query = session.QueryOver<T>();
                    applyFilter?.Invoke(query);
                    return query.RowCount();
                }
                catch (HibernateException e)
                {
                    _log.Error(
                        $"Exception occured when system tried to get the count of {_entityName} from database",
                        e);
                    throw;
                }
            }
        }

        public static T NewEntity()
        {
            return new T();
        }
    }

}
