using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityAccessManager.Identity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource("roles", "User roles", new List<string> { "role" })
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("api1", "My API"),
            new ApiScope("users", "Users API"),
            new ApiScope("notifications", "Notifications API")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // Machine to machine client
            new Client
            {
                ClientId = "client",
                ClientName = "Client Credentials Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "api1" }
            },

            // Interactive ASP.NET Core MVC client
            new Client
            {
                ClientId = "mvc",
                ClientName = "MVC Client",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RequirePkce = true,
                RedirectUris = { "https://localhost:5002/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    "api1"
                }
            },

            // Next.js SPA client with PKCE
            new Client
            {
                ClientId = "nextjs",
                ClientName = "Next.js SPA",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RequirePkce = true,
                RedirectUris = { "http://localhost:3000/callback", "https://localhost:3000/callback" },
                PostLogoutRedirectUris = { "http://localhost:3000", "https://localhost:3000" },
                AllowedCorsOrigins = { "http://localhost:3000", "https://localhost:3000" },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    "api1",
                    "users",
                    "notifications"
                },
                AccessTokenLifetime = 3600, // 1 hour
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                SlidingRefreshTokenLifetime = 2592000 // 30 days
            }
        };
} 