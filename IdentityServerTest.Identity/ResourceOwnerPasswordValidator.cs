using IdentityModel;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServerTest.Identity
{
    // OAuth 2.0 resource owner password credential grant
    // https://identityserver4.readthedocs.io/en/dev/topics/resource_owner.html
    // https://tools.ietf.org/html/rfc6749#section-1.3.3
    // 
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        // Auhtentication method reference "amr":
        // https://tools.ietf.org/html/draft-jones-oauth-amr-values-00#section-7.1
        //
        // Authentication Methods References. JSON array of strings that are identifiers for authentication methods used in
        // the authentication.For instance, values might indicate that both
        // password and OTP authentication methods were used.
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            context.Result = new GrantValidationResult(
                "alice",
                OidcConstants.AuthenticationMethods.Password,
                new List<Claim>());
            return Task.FromResult(0);
        }
    }
}
