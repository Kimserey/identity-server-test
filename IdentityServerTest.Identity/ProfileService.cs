using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;

namespace IdentityServerTest.Identity
{
    public class ProfileService : IProfileService
    {
        private ILogger<ProfileService> _logger;

        public ProfileService(ILogger<ProfileService> logger)
        {
            _logger = logger;
        }

        // Will be consulted after the IsActiveAsync is consulted.
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            _logger.LogDebug("Hello world");
            return Task.FromResult(0);
        }

        // Will be consulted after the Resource Owner password validator returns a valid grant.
        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(0);
        }
    }
}
