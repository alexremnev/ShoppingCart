using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Filters;
using Spring.Context.Support;

namespace ShoppingCart.WebApi
{
    public static class WebApiConfig
    {
        public static readonly string DefaultRoute = "DefaultApi";
        public const string SegmentOfRouteTemplate = "api/";
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
             DefaultRoute,
             SegmentOfRouteTemplate + "{controller}/{id}",
             new { id = RouteParameter.Optional },
             new { id = @"\d+" }
        );
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Filters.Add((IFilter)ContextRegistry.GetContext().GetObject("IdentityBasicAuthenticationAttribute"));
            config.Filters.Add((IFilter)ContextRegistry.GetContext().GetObject("MyAuthorizeAttribute"));
        }
    }
}
