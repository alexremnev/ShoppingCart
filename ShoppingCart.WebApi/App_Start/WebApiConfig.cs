using System.Net.Http.Headers;
using System.Web.Http;
using ShoppingCart.WebApi.Security;

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
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}
