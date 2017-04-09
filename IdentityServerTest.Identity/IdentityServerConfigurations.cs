using System;
using System.Security.Claims;
using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServerTest.Identity
{
	public class IdentityServerConfigurations
	{
		public static IEnumerable<IdentityResource> GetIdentityResources()
		{
			return new List<IdentityResource>
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile()
			};
		}

		public static IEnumerable<ApiResource> GetApiResources()
		{
			return new List<ApiResource>
			{
				new ApiResource("webapi", "Web Api")
			};
		}

		public static IEnumerable<Client> GetClients()
		{
			return new List<Client>
			{
				new Client {
					ClientId = "website_1",
					ClientName = "Website 1",
					AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
					ClientSecrets =
					{
						new Secret("secret".Sha256())
					},
					AllowedScopes =
					{
						"webapi"
					},
					Claims =
					{
						new Claim("site.context", "one")
					}
				},
				new Client {
					ClientId = "website_2",
					ClientName = "website_2",
					RedirectUris = { "http://localhost:5002/signin-oidc" },
					PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
					AllowedGrantTypes = GrantTypes.Implicit,
					AllowedScopes =
					{
						"webapi"
					},
					Claims =
					{
						new Claim("site.context", "two")
					}
				}
			};
		}

		public static List<TestUser> GetTestUsers()
		{
			return new List<TestUser> {
				new TestUser {
					SubjectId = "1",
					Username = "alice",
					Password = "password",
					Claims = 
					{
						new Claim("test", "test")
					}
				},
				new TestUser
				{
					SubjectId = "2",
					Username = "bob",
					Password = "password"
				}
			};
		}
	}
}
