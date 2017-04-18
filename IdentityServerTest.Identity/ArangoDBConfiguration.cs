using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerTest.Identity
{
    public class ArangoDBConfiguration
    {
        public string Url { get; set; }
        public string Database { get; set; }
        public Credentials Credentials { get; set; }
        public ArangoDBCollections Collections { get; set; }
    }

    public class Credentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class ArangoDBCollections
    {
        public string PersistedGrants { get; set; }
    }
}
