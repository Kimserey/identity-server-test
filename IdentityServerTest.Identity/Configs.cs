using System;
using System.Security.Claims;
using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityModel;
using IdentityServer4;
using System.Linq;

namespace IdentityServerTest.Identity
{
    public class Configs
    {
        // Identity resources are retrieved from the UserInfo endpoint.
        // It can be set on the middleware "GetClaimsFromUserInfoEndpoint=true",
        // or can be invoked from the SDK client with UserInfoClient.
        // Place retrievable identity information here.
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                // Every default IdentityResources contain a set of JwtUserClaims which will
                // be added to the token.
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("webapi", "Web Api"),
                new ApiResource("api", "Web Api call")
                {
                    ApiSecrets =
                    {
                        // Api secret is used for the introspect endpoint.
                        // The introspect endpoint is hit by the client to find back
                        // identity using Reference token.
                        // Because the introspect endpoint needs authentication,
                        // the secret is used together with API name.
                        new Secret("secret".Sha256())
                    },
                    Scopes =
                    {
                        new Scope("api.call")
                        {
                            UserClaims = {
                                JwtClaimTypes.Name
                            }
                        },
                        new Scope("api.receive")
                    },
                    UserClaims =
                    {
                        JwtClaimTypes.Name
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword.Concat(new [] { "scoped" }),
                    RequireClientSecret = false,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {
                        "api",
                        "api.call",
                        // OpenId scope must be allowed scope to retrieve Identity claims from UserInfo endpoint.
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email
                    },
                    Claims = {
                        new Claim("client-claim", "test")
                    },
                    AccessTokenType = AccessTokenType.Reference
                },
                new Client {
                    ClientId = "website_1",
                    ClientName = "Website 1",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes =
                    {
                        "api.receive",
                        JwtClaimTypes.Profile,
                    },
                    Claims =
                    {
                        new Claim("site.context", "one")
                    },
                    AccessTokenType = AccessTokenType.Reference
                },
                new Client {
                    ClientId = "website_2",
                    ClientName = "website_2",
                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedScopes =
                    {
                        "webapi",
                        IdentityServerConstants.StandardScopes.Profile
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
                        new Claim("test", "test"),
                        new Claim(JwtClaimTypes.Name, "alice"),
                        new Claim(JwtClaimTypes.NickName, "nickname"),
                        new Claim(JwtClaimTypes.Email, "test1@gmail.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "test2@gmail.com")
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
