namespace ShoppingCart.Business
{
    public interface IAuthorizeService
    {
        string FindPermission(string controllerName, string methodName);
    }
}
