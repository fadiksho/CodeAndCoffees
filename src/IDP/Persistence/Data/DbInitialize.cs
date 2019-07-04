using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IDP.Models;
using IDP.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;

namespace IDP.Persistence.Data
{
  public class DbInitialize
  {
    public static void EnsureSeedData(IServiceProvider provider)
    {
      var env = provider.GetRequiredService<IHostingEnvironment>();
      provider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
      var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
      var config = provider.GetRequiredService<IConfiguration>().Get<AppSettings>();
      var admin = userManager.FindByNameAsync(config.AdminUser.UserName).Result;
      // create an admin user if not exist.
      if (admin == null)
      {
        admin = new ApplicationUser
        {
          UserName = config.AdminUser.UserName
        };
        var result = userManager.CreateAsync(admin, config.AdminUser.Password).Result;
        if (!result.Succeeded)
        {
          throw new Exception(result.Errors.First().Description);
        }

        admin = userManager.FindByNameAsync(config.AdminUser.UserName).Result;

        result = userManager.AddClaimsAsync(admin, new Claim[]{
          new Claim(JwtClaimTypes.Name, config.AdminUser.ClaimName),
          new Claim(JwtClaimTypes.GivenName, config.AdminUser.GivenName),
          new Claim(JwtClaimTypes.Email, config.AdminUser.Email),
          new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
          new Claim(JwtClaimTypes.Role, "admin")
          }).Result;
        if (!result.Succeeded)
        {
          throw new Exception(result.Errors.First().Description);
        }
        Console.WriteLine($"{config.AdminUser.UserName} created");
      }

      // intialize IdentityServer4 clients and resourses in db.
      if (!env.IsDevelopment())
      {
        provider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
        provider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();

        var context = provider.GetRequiredService<ConfigurationDbContext>();
        if (!context.Clients.Any())
        {
          foreach (var client in Config.GetClients())
          {
            context.Clients.Add(client.ToEntity());
          }
          context.SaveChanges();
        }

        if (!context.IdentityResources.Any())
        {
          foreach (var resource in Config.GetIdentityResources())
          {
            context.IdentityResources.Add(resource.ToEntity());
          }
          context.SaveChanges();
        }

        if (!context.ApiResources.Any())
        {
          foreach (var resource in Config.GetApis())
          {
            context.ApiResources.Add(resource.ToEntity());
          }
          context.SaveChanges();
        }
      }
    }
  }
}
