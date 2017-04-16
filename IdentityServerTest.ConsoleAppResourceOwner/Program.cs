using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel.Client;
using System.Net;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace IdentityServerTest.ConsoleAppResourceOwner
{
    class Program
    {
        public static async Task Start()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");

            // Get the token
            //
            Console.WriteLine("Getting token");
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "password");
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }
            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();

            // Query API with access token
            //
            Console.WriteLine("Querying API to get data using token");
            try
            {
                var data = GetData(tokenResponse.AccessToken).Result;
                Console.WriteLine(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();

            // Get identity claims from UserInfo
            //
            Console.WriteLine("Getting UserInfo");
            var extraClaims = new UserInfoClient(disco.UserInfoEndpoint);
            var identityClaims = await extraClaims.GetAsync(tokenResponse.AccessToken);
            if (!tokenResponse.IsError)
            {
                Console.WriteLine(identityClaims.Json);
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        /// <summary>
        /// Api call to the protected resource
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static async Task<string> GetData(string accessToken)
        {
            var protectedUrl = "http://localhost:5001/api/data";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(protectedUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return response.ReasonPhrase;
                }
            }
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Press any key to start...");
                Console.ReadKey();
                try
                {
                    Start().Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
