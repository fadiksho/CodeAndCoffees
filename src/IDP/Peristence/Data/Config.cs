
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

          RedirectUris =           { "http://localhost:4200/callback.html" },
          PostLogoutRedirectUris = { "http://localhost:4200/index.html" },
          AllowedCorsOrigins =     { "http://localhost:4200/" },

          AllowedScopes =
          {
            IdentityServerConstants.StandardScopes.OpenId,
            IdentityServerConstants.StandardScopes.Profile,
            "codeandcoffees.blog.api"
          }
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
