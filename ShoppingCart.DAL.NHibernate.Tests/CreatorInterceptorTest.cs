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
            IChangeableEntity entity=new Product();
            var mock = new Mock<ISecurityContext>();
            mock.Setup(m => m.UserName).Returns(name);
            var state = new[] { new object(), new object() };
            var propertyNames = new[] { "name", Property };
            var interceptor = new CreatorInterceptor(mock.Object);

            var actual = interceptor.OnSave(entity, null, state, propertyNames, null);
            Assert.AreEqual(name, state[1]);
            Assert.IsTrue(actual);
        }
    }
}
