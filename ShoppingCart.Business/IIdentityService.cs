using System.Security.Principal;

namespace ShoppingCart.Business
{
    public interface IIdentityService
    {
        /// <summary>
        /// Assign roles for users
        /// </summary>
        /// <param name="name">user's name</param>
        /// <param name="password">user's password</param>
        /// <returns></returns>
        IPrincipal AssignClaim(string name, string password);
    }
}
