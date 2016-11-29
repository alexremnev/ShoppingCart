using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ShoppingCart.DAL.NHibernate.Tests
{
    [TestClass]
    public class CreatorInterceptorTest
    {
        private const string Property = "Creator";
        [TestMethod]
        public void can_save_user()
        {
            const string name = "Clark";
           // IAuditableEntity entity=new Product();
            var mockAuditableEntity = new Mock<IAuditableEntity>();
            var mockSecutiryContext = new Mock<ISecurityContext>();
            mockSecutiryContext.Setup(m => m.UserName).Returns(name);
            var state = new[] { new object(), new object() };
            var propertyNames = new[] { "name", Property };
            var interceptor = new CreatorInterceptor(mockSecutiryContext.Object);

            var actual = interceptor.OnSave(mockAuditableEntity.Object, null, state, propertyNames, null);
            Assert.AreEqual(name, state[1]);
            Assert.IsTrue(actual);
        }
    }
}
