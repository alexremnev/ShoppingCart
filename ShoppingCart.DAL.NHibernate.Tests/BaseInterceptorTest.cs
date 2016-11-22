using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;

namespace ShoppingCart.DAL.NHibernate.Tests
{
    [TestClass]
    public abstract class BaseInterceptorTest<T> where T : IInterceptor, new()
    {
        private readonly string _property;

        protected BaseInterceptorTest(string property)
        {
            _property = property;
        }
        [TestMethod]
        public void can_save()
        {
            var state = new[] { new object(), new object() };
            var propertyNames = new[] { "name", _property };
            var interceptor = new T();

            var actual = interceptor.OnSave(null, null, state, propertyNames, null);
            Assert.IsTrue(actual);
            Assert.IsInstanceOfType(state[1], typeof(DateTime));
        }
    }
}
