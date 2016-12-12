using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Business;
using ShoppingCart.WebApi.Security;

namespace ShoppingCart.WebApi.Tests
{
    [TestClass]
    public class IdentityBasicAuthenticationAttributeTest
    {
        [TestMethod]
        public void Can_auth_async_with_correct_name_and_password()
        {
            var request = new HttpRequestMessage();
            var controllerContext = new HttpControllerContext { Request = request };
            var context = new HttpActionContext { ControllerContext = controllerContext };
            var headers = request.Headers;
            var authorization = new AuthenticationHeaderValue("Basic", "QWxleDoxMTE=");
            headers.Authorization = authorization;
            var authenticationContext = new HttpAuthenticationContext(context, null);

            var claims = new List<Claim>
                {
               new Claim(ClaimTypes.Name, "Alex"),
               new Claim(ClaimTypes.Role, "111")
                };
            var id = new ClaimsIdentity(claims, "Token");

            var mockService = new Mock<IIdentityService>();
            var mockPrincipal = new Mock<IPrincipal>();
            mockPrincipal.Setup(s => s.Identity).Returns(id);
            mockService.Setup(s => s.AssignClaim("Alex", "111")).Returns(mockPrincipal.Object);
            var attribute = new IdentityBasicAuthenticationAttribute(mockService.Object);
            attribute.AuthenticateAsync(authenticationContext, CancellationToken.None);

            var expected = id;
            Assert.AreEqual(expected, authenticationContext.Principal.Identity);
        }

        [TestMethod]
        public void Can_auth_async_with_empty_username_and_password()
        {
            var request = new HttpRequestMessage();
            var controllerContext = new HttpControllerContext { Request = request };
            var context = new HttpActionContext { ControllerContext = controllerContext };
            var authenticationContext = new HttpAuthenticationContext(context, null);

            var claims = new List<Claim>
                {
               new Claim(ClaimTypes.Name, "Alex"),
               new Claim(ClaimTypes.Role, "111")
                };
            var id = new ClaimsIdentity(claims, "Token");

            var mockService = new Mock<IIdentityService>();
            var mockPrincipal = new Mock<IPrincipal>();
            mockPrincipal.Setup(s => s.Identity).Returns(id);
            mockService.Setup(s => s.AssignClaim("Alex", "111")).Returns(mockPrincipal.Object);
            var attribute = new IdentityBasicAuthenticationAttribute(mockService.Object);
            attribute.AuthenticateAsync(authenticationContext, CancellationToken.None);

            Assert.IsInstanceOfType(authenticationContext.ErrorResult, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void Can_auth_async_with_wrong_username_and_password()
        {
            var request = new HttpRequestMessage();
            var controllerContext = new HttpControllerContext { Request = request };
            var context = new HttpActionContext { ControllerContext = controllerContext };
            var headers = request.Headers;
            var authorization = new AuthenticationHeaderValue("Basic", "qqq");
            headers.Authorization = authorization;
            var authenticationContext = new HttpAuthenticationContext(context, null);

            var claims = new List<Claim>
                {
               new Claim(ClaimTypes.Name, "Alex"),
               new Claim(ClaimTypes.Role, "111")
                };
            var id = new ClaimsIdentity(claims, "Token");

            var mockService = new Mock<IIdentityService>();
            var mockPrincipal = new Mock<IPrincipal>();
            mockPrincipal.Setup(s => s.Identity).Returns(id);
            mockService.Setup(s => s.AssignClaim("Alex", "111")).Returns(mockPrincipal.Object);
            var attribute = new IdentityBasicAuthenticationAttribute(mockService.Object);
            attribute.AuthenticateAsync(authenticationContext, CancellationToken.None);

            Assert.IsInstanceOfType(authenticationContext.ErrorResult, typeof(UnauthorizedResult));
        }

        //[TestMethod]
        //public void can_challaegeAsync()
        //{
        //    var request = new HttpRequestMessage();
        //    var controllerContext = new HttpControllerContext { Request = request };
        //    var actionContext = new HttpActionContext { ControllerContext = controllerContext };
        //    var actionResult = (IHttpActionResult)new UnauthorizedResult(new AuthenticationHeaderValue[0], actionContext.Request);
        //    var context = new HttpAuthenticationChallengeContext(actionContext, actionResult);
        //    var expectedChallenge = new AuthenticationHeaderValue("Basic");
        //    var expected = new AddChallengeOnUnauthorizedResult(expectedChallenge, context.Result);
        //    //var expected = context;

        //    var mock = new Mock<IIdentityService>();
        //    var attribute = new IdentityBasicAuthenticationAttribute(mock.Object);
        //    attribute.ChallengeAsync(context, CancellationToken.None);

        //    Assert.AreEqual(expected.Challenge, context.Result);
        //    //            Assert.AreNotEqual(expected.Result, context.Result);

        //}
    }
}
