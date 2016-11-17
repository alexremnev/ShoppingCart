using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShoppingCart.DAL.NHibernate.Tests
{
	[TestClass]
	public class ModifiedDateInterceptorTest
	{
		[TestMethod]
		public void can_modifiedDate_save()
		{
            var state = new[] { new object(), new object() };
            const string property = "ModifiedDate";
            var propertynames = new[] { "name", property };
            var interceptor = new ModifiedDateInterceptor();

            var actual = interceptor.OnSave(null, null, state, propertynames, null);
            Assert.IsTrue(actual);
            Assert.IsNotNull(DateTime.Parse(state[1].ToString()));
        }

        [TestMethod]
        public void can_modifiedDate_update()
        {
            var previousState = default(DateTime);
            var currentState = new[] { new object(), previousState };
            const string property = "ModifiedDate";
            var propertynames = new[] { "name", property };
            var interceptor = new ModifiedDateInterceptor();

            var actual = interceptor.OnFlushDirty(null, null, currentState, null, propertynames, null);
            Assert.IsTrue(actual);
            Assert.AreNotEqual(previousState,DateTime.Parse(currentState[1].ToString()));
        }
    }
}
