using System;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace IdentityServerTest.ConsoleAppResourceOwner
{
	class Program
    {
        // Reference token example
        // {
        //    "access_token": "923da74fd0ce0c6ec67a75e5d5fbfae25911ac639acbcc270d34d6ecf652f641",
        //    "expires_in": 3600,
        //    "token_type": "Bearer"
        // }
        public static async Task GetToken()
		{
			var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
			// request token
			var tokenClient = new TokenClient(disco.TokenEndpoint, "website_call", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "password");
            
            if (tokenResponse.IsError)
			{
				Console.WriteLine(tokenResponse.Error);
				return;
			}

			Console.WriteLine(tokenResponse.Json);

            var extraClaims = new UserInfoClient(disco.UserInfoEndpoint);
            var identityClaims = await extraClaims.GetAsync(tokenResponse.AccessToken);

            if (!tokenResponse.IsError)
            {
                Console.WriteLine(identityClaims.Json);
            }
        }

        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    GetToken().Wait();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Console.ReadKey();
            }
        }
	}
}
