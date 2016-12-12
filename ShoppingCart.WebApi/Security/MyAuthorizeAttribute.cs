using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using ShoppingCart.Business;

namespace ShoppingCart.WebApi.Security
{
    public class MyAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly IAuthorizeService _service;

        public MyAuthorizeAttribute(IAuthorizeService service)
        {
            _service = service;
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var principal = actionContext.ControllerContext.RequestContext.Principal;
            var controllerName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
            var methodName = actionContext.ActionDescriptor.ActionName;
            var rolesString = _service.FindPermission(controllerName, methodName);
            var roles = SplitString(rolesString);
            return roles.Any(role => principal.IsInRole(role));
        }

        private static readonly string[] Array = new string[0];

        internal static string[] SplitString(string original)
        {
            if (string.IsNullOrEmpty(original))
                return Array;
            return original.Split(',').Select(piece => new
            {
                piece = piece,
                trimmed = piece.Trim()
            }).Where(param0 => !string.IsNullOrEmpty(param0.trimmed)).Select(param0 => param0.trimmed).ToArray();
        }
    }
}