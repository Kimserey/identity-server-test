using System.Threading.Tasks;
using IdentityServer4.Validation;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using IdentityModel;
using Microsoft.Extensions.Options;

namespace IdentityServerTest.Identity
{
    public class CustomGrantValidatorOptions
    {
        // Scope allowed to request for limited access token
        public string Scope { get; set; }
    }

    public class CustomGrantValidator : IExtensionGrantValidator
    {
        private ITokenValidator _tokenValidator;
        private CustomGrantValidatorOptions _options;

        public CustomGrantValidator(IOptions<CustomGrantValidatorOptions> options, ITokenValidator tokenValidator)
        {
            _options = options.Value;
            _tokenValidator = tokenValidator;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var token = context.Request.Raw.Get("access_token");
            var area = context.Request.Raw.Get("area");

            var result = await _tokenValidator.ValidateAccessTokenAsync(token);

            if (result.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid grant");
                return;
            }

            var scope = result.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Scope && c.Value == _options.Scope);

            if (scope == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid grant");
                return;
            }

            context.Result = new GrantValidationResult(subject: "alice", authenticationMethod: "scoped", claims: new List<Claim> { new Claim("limited", $"area:{area}") });
        }

        public string GrantType
        {
            get { return "scoped"; }
        }
    }
}
