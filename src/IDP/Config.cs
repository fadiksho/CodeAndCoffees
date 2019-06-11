
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IDP
{
  public class Config
  {
    public static List<TestUser> GetUsers()
    {
      return new List<TestUser>
      {
        new TestUser
        {
          SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
          Username = "Fadi",
          Password = "password",
          Claims = new List<Claim>
          {
            new Claim("given_name", "Fadi"),
            new Claim("family_name", "Ksho")
          }
        }
      };
    }

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
      return new List<Client>();
    }
  }
}
