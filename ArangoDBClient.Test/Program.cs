using ArangoDB.Client;
using ArangoDB.Client.Property;
using ArangoDB.Client.Utility;
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

        public static string ResolveId(string id, string collectionName = null)
        {
            return id.IndexOf("/") == -1 ? $"{Encode(collectionName)}/{Encode(id)}" : Encode(id);
        }

        public static string Encode(string s)
        {
            return WebUtility.UrlEncode(s);
        }

        // Logic extracted from Owin Katana Base64UrlTextEncoder 
        // https://github.com/yreynhout/katana-clone/blob/master/src/Microsoft.Owin.Security/DataHandler/Encoder/Base64UrlTextEncoder.cs
        //
        public static string ToBase64UrlFromString(string value)
        {
            return Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("hello/world"))
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
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

            // Keys must be saved without encoding but
            // retrieved with UrlEncoding.
            //
            using (var database = ArangoDatabase.CreateWithSetting())
            {
                database.Collection("Hello").Insert(new Hello { Key = ToBase64UrlFromString("hello/world"), Text = "Hello world" });
                var data = database.Collection("Hello").Document<Hello>(ToBase64UrlFromString("hello/world"));

                //var grant = database.Collection("PersistedGrants").Document<PersistedGrants>(WebUtility.UrlEncode("gmzAS+Gw3zjqPNU0sFvRBC9AGkZqXyNxRx+HQhPiUvs="));
                //Console.WriteLine(grant.Key);
            }
        }
    }
}