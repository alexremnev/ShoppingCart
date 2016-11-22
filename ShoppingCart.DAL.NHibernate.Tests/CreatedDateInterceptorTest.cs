using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShoppingCart.DAL.NHibernate.Tests
{
    [TestClass]
    public class CreatedDateInterceptorTest : BaseInterceptorTest<CreatedDateInterceptor>
    {
        public CreatedDateInterceptorTest() : base("CreatedDate") { }

        [TestMethod]
        public void can_createdDate_save()
        {
            can_save();
        }
    }
}
