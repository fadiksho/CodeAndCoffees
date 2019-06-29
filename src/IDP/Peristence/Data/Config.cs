using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IDP.Peristence.Data
{
  public class Config
  {
    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
      return new List<IdentityResource>
      {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
      };
    }

    public static IEnumerable<Client> GetClients()
    {
      return new List<Client>()
      {
        new Client
        {
          ClientId = "blogManager_SPA",
          ClientName = "Blog Manager SPA Client",
          AllowedGrantTypes = GrantTypes.Code,
          RequirePkce = true,
          RequireClientSecret = false,

          RedirectUris =           { "http://localhost:4200/auth-callback", "http://localhost:4200/silent-refresh.html" },
          PostLogoutRedirectUris = { "http://localhost:4200/?postLogout=true" },
          AllowedCorsOrigins =     { "http://localhost:4200" },
          RequireConsent = false,
          AllowRememberConsent = false,
          AllowedScopes =
          {
            IdentityServerConstants.StandardScopes.OpenId,
            IdentityServerConstants.StandardScopes.Profile,
            "codeandcoffees.blog.api"
          },
          AllowAccessTokensViaBrowser = true,
          
          // 30 Minutes
          AccessTokenLifetime = 1800,
        }
      };
    }

    public static IEnumerable<ApiResource> GetApis()
    {
      return new List<ApiResource>
      {
        new ApiResource("codeandcoffees.blog.api", "Code And Coffees Blog API")
      };
    }
  }
}
