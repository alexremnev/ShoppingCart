using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShoppingCart.DAL.NHibernate.Tests
{
    [TestClass]
    public class CreatedDateInterceptorTest
    {
        [TestMethod]
        public void can_createdDate_save()
        {
            var state = new[] { new object(), new object() };
            const string property = "CreatedDate";
            var propertynames = new[] { "name", property };
            var interceptor = new CreatedDateInterceptor();

            var actual = interceptor.OnSave(null, null, state, propertynames, null);
            Assert.IsTrue(actual);
            Assert.IsNotNull(DateTime.Parse(state[1].ToString()));
        }
    }
}
