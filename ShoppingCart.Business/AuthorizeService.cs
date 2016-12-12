using ShoppingCart.DAL;

namespace ShoppingCart.Business
{
    public class AuthorizeService : IAuthorizeService
    {
        private readonly IAuthorizeRepository _repository;
        public AuthorizeService(IAuthorizeRepository repository)
        {
            _repository = repository;
        }

        public string FindPermission(string controllerName, string methodName)
        {
            var controllersNameList = _repository.List();

            for (var i = 0; i < controllersNameList.Count - 1; i++)
            {
                if (controllersNameList[i].Name == controllerName)
                {
                    for (var j = 0; j < controllersNameList[i].Methods.Count; j++)
                    {
                        if (controllersNameList[i].Methods[j].Name == methodName)
                        {
                            return controllersNameList[i].Methods[j].Roles;
                        }
                    }
                }
            }
            return null;
        }
    }
}
