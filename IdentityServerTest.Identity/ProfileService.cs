using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace IdentityServerTest.Identity
{
    public class ProfileService : IProfileService
    {
        private ILogger _logger;

        public ProfileService(ILogger<ProfileService> logger)
        {
            _logger = logger;
        }

        // Will be consulted after the IsActiveAsync is consulted.
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            _logger.LogDebug("Get profile data");

            // Filters the list of claims given and returns 
            // only the requested claims available in the claims given.
            // The requested claims is fill up from the ApiResource.UserClaims.
            context.AddFilteredClaims(new List<Claim>());

            return Task.FromResult(0);
        }

        // Will be consulted after the Resource Owner password validator returns a valid grant.
        public Task IsActiveAsync(IsActiveContext context)
        {
            _logger.LogDebug("Check if user is active");

            context.IsActive = true;
            return Task.FromResult(0);
        }
    }
}
