using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using ShoppingCart.Business;
using static System.String;

namespace ShoppingCart.WebApi.Security
{
    public class IdentityBasicAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public IIdentityService Service { get; set; }
        public IdentityBasicAuthenticationAttribute(IIdentityService service)
        {
            Service = service;
        }

        public bool AllowMultiple => false;
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var req = context.Request;
            if (req.Headers.Authorization == null || !"Basic".Equals(req.Headers.Authorization.Scheme,
                    StringComparison.OrdinalIgnoreCase))
            {
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                return Task.FromResult(0);
            }

            var userNameAndPasword = ExtractUserNameAndPassword(req.Headers.Authorization.Parameter) ?? new Tuple<string, string>(Empty, Empty);
            var userName = userNameAndPasword.Item1;
            var password = userNameAndPasword.Item2;
            if (!IsNullOrEmpty(userName) || !IsNullOrEmpty(password))
            {
                var principal = Service.AssignClaim(userName, password);
                context.Principal = principal;
            }
            else
            {
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
            }
            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            //var challenge = new AuthenticationHeaderValue("Basic");
            //context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
            return Task.FromResult(0);
        }

        private static Tuple<string, string> ExtractUserNameAndPassword(string authorizationParameter)
        {
            byte[] credentialBytes;

            try
            {
                credentialBytes = Convert.FromBase64String(authorizationParameter);
            }
            catch (FormatException)
            {
                return null;
            }

            var encoding = Encoding.ASCII;
            encoding = (Encoding)encoding.Clone();
            encoding.DecoderFallback = DecoderFallback.ExceptionFallback;
            string decodedCredentials;

            try
            {
                decodedCredentials = encoding.GetString(credentialBytes);
            }
            catch (DecoderFallbackException)
            {
                return null;
            }

            if (IsNullOrEmpty(decodedCredentials))
            {
                return null;
            }

            var colonIndex = decodedCredentials.IndexOf(':');

            if (colonIndex == -1)
            {
                return null;
            }

            var userName = decodedCredentials.Substring(0, colonIndex);
            var password = decodedCredentials.Substring(colonIndex + 1);
            return new Tuple<string, string>(userName, password);
        }
    }
}




