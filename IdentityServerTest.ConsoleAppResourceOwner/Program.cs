using System;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace IdentityServerTest.ConsoleAppResourceOwner
{
	class Program
	{
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
		}
	}
}
