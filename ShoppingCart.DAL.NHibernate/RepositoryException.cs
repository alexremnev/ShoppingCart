using System;

namespace ShoppingCart.DAL.NHibernate
{
    public class RepositoryException : InternalException
    {
        public RepositoryException() { }
        public RepositoryException(string message) : base(message) { }
        public RepositoryException(string message, Exception inner) : base(message, inner) { }
    }
}