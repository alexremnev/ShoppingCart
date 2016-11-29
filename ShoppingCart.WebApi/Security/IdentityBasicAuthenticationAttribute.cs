using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using static System.String;

namespace ShoppingCart.WebApi.Security
{
    public class IdentityBasicAuthenticationAttribute : Attribute, IAuthenticationFilter

    {
        public bool AllowMultiple => false;
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var req = context.Request;

            if (req.Headers.Authorization == null || !"Basic".Equals(req.Headers.Authorization.Scheme,
                    StringComparison.OrdinalIgnoreCase)) return Task.FromResult(0);
            // var creds = req.Headers.Authorization.Parameter;
            //   if (creds != null)// if (creds == "QWxleDpSZW1uZXY=")
            //   {
            var userNameAndPasword = ExtractUserNameAndPassword(req.Headers.Authorization.Parameter);
            var userName = userNameAndPasword.Item1;
            if (!IsNullOrEmpty(userName))
            {
                // var password = userNameAndPasword.Item2;
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,userName),
                    new Claim(ClaimTypes.Role, "admin")
                };
                var id = new ClaimsIdentity(claims, "Token");
                var principal = new ClaimsPrincipal(new[] { id });
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
            var challenge = new AuthenticationHeaderValue("Basic");
            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
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

    public class AddChallengeOnUnauthorizedResult : IHttpActionResult
    {
        public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IHttpActionResult innerResult)
        {
            Challenge = challenge;
            InnerResult = innerResult;
        }

        public AuthenticationHeaderValue Challenge { get; }

        public IHttpActionResult InnerResult { get; }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await InnerResult.ExecuteAsync(cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized) return response;
            if (response.Headers.WwwAuthenticate.All(h => h.Scheme != Challenge.Scheme))
            {
                response.Headers.WwwAuthenticate.Add(Challenge);
            }

            return response;
        }
    }
}




