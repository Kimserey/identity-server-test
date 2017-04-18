using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ArangoDB.Client;
using System.Net;

namespace IdentityServerTest.Identity
{
    // Using ArangoDB.Client to store Access token linking to Reference token
    //
    public class ArangoPersistedGrantStore : IPersistedGrantStore
    {
        private class ArangoDBPersistedGrant
        {
            [DocumentProperty(Identifier = IdentifierType.Key)]
            public string Key { get; set; }
            public string Type { get; set; }
            public string SubjectId { get; set; }
            public string ClientId { get; set; }
            public DateTime CreationTime { get; set; }
            public DateTime? Expiration { get; set; }
            public string Data { get; set; }
        }

        private ILogger<ArangoPersistedGrantStore> _logger;
        private ArangoDBConfiguration _arangoDBConfig;

        public ArangoPersistedGrantStore(ILogger<ArangoPersistedGrantStore> logger, IOptions<ArangoDBConfiguration> arangoDBConfig)
        {
            _logger = logger;
            _arangoDBConfig = arangoDBConfig.Value;

            var creds = new NetworkCredential(
                _arangoDBConfig.Credentials.UserName,
                _arangoDBConfig.Credentials.Password
            );

            ArangoDatabase.ChangeSetting(s =>
            {
                s.Database = arangoDBConfig.Value.Database;
                s.Url = arangoDBConfig.Value.Url;
                s.Credential = creds;
                s.SystemDatabaseCredential = creds;
            });
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            using (var database = ArangoDatabase.CreateWithSetting())
            {
                return await database.Query<ArangoDBPersistedGrant>()
                    .Where(pg => pg.SubjectId == subjectId)
                    .Select(grant => new PersistedGrant
                    {
                        Key = grant.Key,
                        ClientId = grant.ClientId,
                        CreationTime = grant.CreationTime,
                        Data = grant.Data,
                        Expiration = grant.Expiration,
                        SubjectId = grant.SubjectId,
                        Type = grant.Type
                    })
                    .ToListAsync();
            }
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            using (var database = ArangoDatabase.CreateWithSetting())
            {
                var grant = await database.Collection(_arangoDBConfig.Collections.PersistedGrants)
                    .DocumentAsync<ArangoDBPersistedGrant>(key);

                return new PersistedGrant
                {
                    Key = grant.Key,
                    ClientId = grant.ClientId,
                    CreationTime = grant.CreationTime,
                    Data = grant.Data,
                    Expiration = grant.Expiration,
                    SubjectId = grant.SubjectId,
                    Type = grant.Type
                };
            }
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            using (var database = ArangoDatabase.CreateWithSetting())
            {
                await database.Query<ArangoDBPersistedGrant>()
                    .Where(pg => pg.SubjectId == subjectId && pg.ClientId == clientId)
                    .Remove()
                    .ToListAsync();
            }
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            using (var database = ArangoDatabase.CreateWithSetting())
            {
                await database.Query<ArangoDBPersistedGrant>()
                    .Where(pg => pg.SubjectId == subjectId && pg.ClientId == clientId && pg.Type == type)
                    .Remove()
                    .ToListAsync();
            }
        }

        public async Task RemoveAsync(string key)
        {
            using (var database = ArangoDatabase.CreateWithSetting())
            {
                var encodedKey = WebUtility.UrlEncode(key);
                await database.Collection(_arangoDBConfig.Collections.PersistedGrants)
                    .RemoveByIdAsync(encodedKey);
            }
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            using (var database = ArangoDatabase.CreateWithSetting())
            {
                await database.Collection(_arangoDBConfig.Collections.PersistedGrants)
                    .InsertAsync(
                        new ArangoDBPersistedGrant {
                            Key = grant.Key,
                            ClientId = grant.ClientId,
                            CreationTime = grant.CreationTime,
                            Data = grant.Data,
                            Expiration = grant.Expiration,
                            SubjectId = grant.SubjectId,
                            Type = grant.Type
                        }
                    );
            }
        }
    }
}
