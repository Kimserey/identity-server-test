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
        // This is requested when token endpoint is invoked for Api resources required claims,
        // and when User info endpoint is invoked for Identity resources required claims.
        // The identity claims are separated from the Api resources claims to keep the token small.
        // If a claim, e.g "name", is vitale for the Api and needs to be in every access token, it can be added in
        // the UserClaims on the ApiResource configuration. This will ensure that on token request, the claim will be present.
        //
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
