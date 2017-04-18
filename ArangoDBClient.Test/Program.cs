using ArangoDB.Client;
using ArangoDB.Client.Property;
using System;
using System.Net;

namespace ArangoDBClient.Test
{
    class Program
    {
        private class PersistedGrants
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

        public class Hello
        {
            [DocumentProperty(Identifier = IdentifierType.Key)]
            public string Key { get; set; }
            public string Text { get; set; }
        }

        static void Main(string[] args)
        {
            var creds = new NetworkCredential("root", "123456");
            ArangoDatabase.ChangeSetting(s =>
            {
                s.Database = "IdentityTest";
                s.Url = "http://localhost:8529/";
                s.Credential = creds;
                s.SystemDatabaseCredential = creds;
            });

            using (var database = ArangoDatabase.CreateWithSetting())
            {
                database.Collection("Hello").Insert(new Hello { Key = "+++" });
                var data = database.Collection("Hello").Document<Hello>("+++");

                //var grant = database.Document<PersistedGrants>("gmzAS+Gw3zjqPNU0sFvRBC9AGkZqXyNxRx+HQhPiUvs=");
                //Console.WriteLine(WebUtility.UrlEncode("gmzAS+Gw3zjqPNU0sFvRBC9AGkZqXyNxRx+HQhPiUvs="));
                //Console.WriteLine(grant.Key);
            }
        }
    }
}