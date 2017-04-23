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
            // The key will be an opaque key which we will use internally only
            [DocumentProperty(Identifier = IdentifierType.Key)]
            public string Key { get; set; }
            public string GrantKey { get; set; }
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

        // Logic extracted from Owin Katana Base64UrlTextEncoder 
        // https://github.com/yreynhout/katana-clone/blob/master/src/Microsoft.Owin.Security/DataHandler/Encoder/Base64UrlTextEncoder.cs
        // We need to use base 64 because the grant key might contain slashes which is not supported by Arango.
        // We can't simply 'url encode' the key and save the document under the encoded key 
        // because when retrieving the data, Arango will try to decode the key which will result in the 'decoded' key checked against the 'encoded' key (saved in database).
        //
        // We only need to encode the value to use it as key - decoding is not needed as we hold the original value in the data.
        private static string ToBase64UrlFromString(string value)
        {
            return Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("hello/world"))
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            using (var database = ArangoDatabase.CreateWithSetting())
            {
                return await database.Query<ArangoDBPersistedGrant>()
                    .Where(pg => pg.SubjectId == subjectId)
                    .Select(grant => new PersistedGrant
                    {
                        Key = grant.GrantKey,
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
                var encodedKey = ToBase64UrlFromString(key);

                var grant = await database.Collection(_arangoDBConfig.Collections.PersistedGrants)
                    .DocumentAsync<ArangoDBPersistedGrant>(encodedKey);

                return new PersistedGrant
                {
                    Key = grant.GrantKey,
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
                var collection = database.Collection(_arangoDBConfig.Collections.PersistedGrants);
                var encodedKey = ToBase64UrlFromString(grant.Key);

                if (await collection.ExistsAsync(encodedKey))
                    return;

                await collection
                    .InsertAsync(
                        new ArangoDBPersistedGrant {
                            Key = encodedKey,
                            GrantKey = grant.Key,
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
