using System.Web.Http;
using ShoppingCart.Business;

namespace ShoppingCart.WebApi.Controllers
{
    public abstract class BaseController<T> : ApiController where T : class
    {
        private const int StartPointPage = 1;
        private const int MaxPageSize = 50;

        public IHttpActionResult List(int? page = StartPointPage, int? pageSize = MaxPageSize)
        {
            var service = GetService();
            page = page ?? StartPointPage;
            page = page < 1 ? StartPointPage : page;
            pageSize = pageSize ?? MaxPageSize;
            pageSize = pageSize > 250 ? MaxPageSize : pageSize;
            pageSize = pageSize <= 0 ? MaxPageSize : pageSize;
            var firstResult = (page - 1) * pageSize;
            var entity = service.List(firstResult.Value, pageSize.Value);
            return Ok(entity);
        }

        public IHttpActionResult Get(int id)
        {
            var service = GetService();
            if (id < 0) return BadRequest();
            var entity = service.Get(id);
            return entity == null ? (IHttpActionResult)NotFound() : Ok(entity);
        }

        public abstract IService<T> GetService();
    }
}
