using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;

namespace IdentityServerTest.Identity
{
    public class ArangoPersistedGrandStore : IPersistedGrantStore
    {
        private ILogger<ArangoPersistedGrandStore> _logger;

        public ArangoPersistedGrandStore(ILogger<ArangoPersistedGrandStore> logger)
        {
            _logger = logger;
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            _logger.LogDebug("GetAllAsync");
            throw new NotImplementedException();
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            _logger.LogDebug("GetAsync");
            throw new NotImplementedException();
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            _logger.LogDebug("RemoveAllAsync");
            throw new NotImplementedException();
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            _logger.LogDebug("RemoveAllAsync");
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string key)
        {
            _logger.LogDebug("RemoveAsync");
            throw new NotImplementedException();
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            _logger.LogDebug("StoreAsync");
            throw new NotImplementedException();
        }
    }
}
