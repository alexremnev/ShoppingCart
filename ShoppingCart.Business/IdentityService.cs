using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using ShoppingCart.DAL;

namespace ShoppingCart.Business
{
    public class IdentityService : IIdentityService
    {
        private readonly IIdentityRepository _repository;
        public IdentityService(IIdentityRepository repository)
        {
            _repository = repository;
        }

        public IPrincipal AssignClaim(string name, string password)
        {
            List<Claim> claims;
            ClaimsIdentity id;
            var user = _repository.FindIdentity(name, password);
            if (user == null)
            {
                var identity = new Identity
                {
                    UserName = name,
                    Password = password,
                    Role = "user"
                };

                _repository.Create(identity);
                claims = new List<Claim>
                {
                    //new Claim(ClaimTypes.Name, identity.UserName),
                    new Claim(ClaimTypes.Role, identity.Role)
                   
                };
                id = new ClaimsIdentity(claims, "Token");
                return new ClaimsPrincipal(new[] { id });
            }

            claims = new List<Claim>
                {
               //new Claim(ClaimTypes.Name, user.UserName),
               new Claim(ClaimTypes.Role, user.Role)
                };
            id = new ClaimsIdentity(claims, "Token");
            return new ClaimsPrincipal(new[] { id });
           }

    }
}
