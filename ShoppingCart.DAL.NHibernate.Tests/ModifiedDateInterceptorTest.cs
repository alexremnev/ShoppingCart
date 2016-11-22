using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShoppingCart.DAL.NHibernate.Tests
{
    [TestClass]
    public class ModifiedDateInterceptorTest : BaseInterceptorTest<ModifiedDateInterceptor>
    {
        private const string Property = "ModifiedDate";
        public ModifiedDateInterceptorTest() : base(Property) { }

        [TestMethod]
        public void can_modifiedDate_save()
        {
            can_save();
        }

        [TestMethod]
        public void can_modifiedDate_update()
        {
            var previousState = default(DateTime);
            var currentState = new[] { new object(), previousState };
            var propertyNames = new[] { "name", Property };
            var interceptor = new ModifiedDateInterceptor();

            var actual = interceptor.OnFlushDirty(null, null, currentState, null, propertyNames, null);
            Assert.IsTrue(actual);
            Assert.AreNotEqual(previousState, (DateTime)currentState[1]);
        }
    }
}
