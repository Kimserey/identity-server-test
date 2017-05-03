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
    public class CustomGrantTypes
    {
        public static string LimitedAccess => "limited_access";
    }

    public class LimitedAccessGrantValidatorOptions
    {
        /// Valid scopes which can be used for limited access grant authorization.
        public IEnumerable<string> ValidScopes { get; set; }
    }

    public class LimitedAccessGrantValidator : IExtensionGrantValidator
    {
        private ITokenValidator _tokenValidator;
        private LimitedAccessGrantValidatorOptions _options;

        public LimitedAccessGrantValidator(IOptions<LimitedAccessGrantValidatorOptions> options, ITokenValidator tokenValidator)
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

            var scope = result.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Scope && _options.ValidScopes.Contains(c.Value));

            if (scope == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid grant");
                return;
            }

            context.Result = new GrantValidationResult(
                subject: result.Claims.FirstOrDefault(claim => claim.Type == JwtClaimTypes.Subject)?.Value,
                authenticationMethod: CustomGrantTypes.LimitedAccess,
                claims: new List<Claim> {
                    new Claim("limit", area)
                });
        }

        public string GrantType
        {
            get { return CustomGrantTypes.LimitedAccess; }
        }
    }
}
