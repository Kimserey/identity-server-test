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
			var tokenClient = new TokenClient(disco.TokenEndpoint, "website_1", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "password");

			if (tokenResponse.IsError)
			{
				Console.WriteLine(tokenResponse.Error);
				return;
			}

			Console.WriteLine(tokenResponse.Json);
		}

		static void Main(string[] args)
		{
			GetToken().Wait();
            Console.ReadKey();
		}
	}
}
