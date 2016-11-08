using System;
using System.Collections.Generic;
using Common.Logging;
using NHibernate;
using Spring.Data.NHibernate;
using Spring.Transaction.Interceptor;

namespace ShoppingCart.DAL.NHibernate
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        public ISessionFactory Sessionfactory { get; set; }
        private const int DefaultFirstResult = 0;
        private const int DefaultMaxResult = 50;

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
                GetLog().Error($"Exception occured when system tried to create an {GetNameOfEntity()}", e);
                throw;
            }
        }

        public IList<T> List(int firstResult = 0, int maxResults = 50)
        {
            if (firstResult < 0) firstResult = DefaultFirstResult;
            if (maxResults < 0) maxResults = DefaultMaxResult;
            if (maxResults == 0) return new List<T>();
            try
            {
                using (var session = Sessionfactory.OpenSession())
                {
                    var query = session.QueryOver<T>();
                    var list = query.Skip(firstResult).Take(maxResults).List();
                    return list;
                }
            }
            catch (Exception e)
            {
                GetLog().Error($"Exception occured when you tried to get the list of {GetNameOfEntity()}s from database", e);
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
                    GetLog().Error(
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
                GetLog().Error($"Exception occured when system tried to update {GetNameOfEntity()}", e);
                throw;
            }
        }

        protected abstract ILog GetLog();
        protected abstract string GetNameOfEntity();
    }
}
