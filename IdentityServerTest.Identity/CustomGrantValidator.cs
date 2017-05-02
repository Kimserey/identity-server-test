using System.Threading.Tasks;
using IdentityServer4.Validation;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServerTest.Identity
{
    public class CustomGrantValidator : IExtensionGrantValidator
    {
        private ITokenValidator _tokenValidator;

        public CustomGrantValidator(ITokenValidator tokenValidator)
        {
            _tokenValidator = tokenValidator;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var token = context.Request.Raw.Get("access_token");
            var area = context.Request.Raw.Get("area");

            var tokenValidationResult = await _tokenValidator.ValidateAccessTokenAsync(token);
;
            if (!tokenValidationResult.IsError)
            {
                context.Result = new GrantValidationResult(subject: "alice", authenticationMethod: "scoped", claims: new List<Claim> { new Claim("limited", $"area:{area}")  });
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
            }
        }

        public string GrantType
        {
            get { return "scoped"; }
        }
    }
}
