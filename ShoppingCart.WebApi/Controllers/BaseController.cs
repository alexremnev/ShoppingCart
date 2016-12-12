using System;
using System.Text;
using System.Web.Http;
using Common.Logging;
using ShoppingCart.Business;
using ShoppingCart.DAL.NHibernate;

namespace ShoppingCart.WebApi.Controllers
{
    public abstract class BaseController<T> : ApiController where T : class
    {
        private const int FirstPage = 1;
        private const int MaxPageSize = 50;
        private const bool DefaultSortDirection = true;
        private readonly string _entityName;
        private readonly ILog _log;

        protected BaseController(string entityName, ISecurityContext context)
        {
            _log = LogManager.GetLogger(GetType());
            _entityName = entityName;
            context.UserName = GenerateName();
        }

        public IHttpActionResult List(string filter = null, string sortby = null, bool? sortDirection = DefaultSortDirection, int? page = FirstPage, int? pageSize = MaxPageSize, string usename = null)
        {
            try
            {
                var service = GetService();
                page = page ?? FirstPage;
                page = page < 1 ? FirstPage : page;
                pageSize = pageSize ?? MaxPageSize;
                pageSize = pageSize > 250 ? MaxPageSize : pageSize;
                pageSize = pageSize <= 0 ? MaxPageSize : pageSize;
                var firstResult = (page - 1) * pageSize;
                sortDirection = sortDirection ?? DefaultSortDirection;
                var entity = service.List(filter, sortby, sortDirection.Value, firstResult.Value, pageSize.Value, usename);
                return Ok(entity);
            }
            catch (Exception e)
            {
                _log.Error($"Exception occured when you tried to get the {_entityName} list", e);
                return InternalServerError();
            }
        }
       
        public IHttpActionResult Get(int id)
        {
            try
            {
                var service = GetService();
                if (id < 0) return BadRequest();
                var entity = service.Get(id);
                return entity == null ? (IHttpActionResult)NotFound() : Ok(entity);
            }
            catch (Exception e)
            {
                _log.Error($"Exception occured when you tried to get the {_entityName} by id={id}", e);
                return InternalServerError();
            }
        }

        public IHttpActionResult Count(string filter = null, decimal maxPrice = 0)
        {
            try
            {
                var count = GetService().Count(filter, maxPrice);
                return Ok(count);
            }
            catch (Exception e)
            {
                _log.Error($"Exception occured when you tried to get {_entityName} count by filter={filter}", e);
                return InternalServerError();
            }
        }
        protected abstract IService<T> GetService();

        protected string GenerateName()
        {
            var r = new Random();
            var userName = new StringBuilder();
            userName.Append((char)r.Next(65, 90));
            var nameLength = r.Next(4, 9);
            for (var i = 0; i < nameLength; i++)
            {
                userName.Append((char)r.Next(97, 122));
            }
            return userName.ToString();
        }
    }
}
