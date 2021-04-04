using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IDP.Models;
using IDP.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;

namespace IDP.Persistence.Data
{
  public class DbInitialize
  {
    public static void EnsureSeedData(IServiceProvider provider)
    {
      provider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
      var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
      var config = provider.GetRequiredService<IConfiguration>().Get<AppSettings>();
      var admin = userManager.FindByNameAsync(config.AdminUser.UserName).Result;
      IdentityResult result;

      if (admin != null)
      {
        result = userManager.DeleteAsync(admin).Result;
        if (!result.Succeeded)
          throw new Exception(result.Errors.First().Description);
      }

      result = userManager.CreateAsync(new ApplicationUser { UserName = config.AdminUser.UserName }, config.AdminUser.Password).Result;

      if (result.Succeeded)
      {
        admin = userManager.FindByNameAsync(config.AdminUser.UserName).Result;

        result = userManager.AddClaimsAsync(admin, new Claim[]{
                new Claim(JwtClaimTypes.Name, config.AdminUser.ClaimName),
                new Claim(JwtClaimTypes.GivenName, config.AdminUser.GivenName),
                new Claim(JwtClaimTypes.Email, config.AdminUser.Email),
                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                new Claim(JwtClaimTypes.Role, "admin")
            }).Result;

        if (result.Succeeded)
          return;
      }

      throw new Exception(result.Errors.First().Description);
    }

    public static void EnsureIdentityServerDatabase(IServiceProvider provider)
    {
      var logger = provider.GetRequiredService<ILogger<DbInitialize>>();

      logger.LogInformation("Try migrating PersistedGrantDbContext");
      provider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

      logger.LogInformation("Try migrating ConfigurationDbContext");
      provider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();

      var context = provider.GetRequiredService<ConfigurationDbContext>();

      logger.LogInformation($"Removing {context.Clients.Count()} Clients.");
      foreach (var client in context.Clients)
      {
        context.Clients.Remove(client);
      }
      context.SaveChanges();
      logger.LogInformation($"Saved.");

      logger.LogInformation($"Adding {Config.GetClients().Count()} clients.");
      foreach (var client in Config.GetClients())
      {
        context.Clients.Add(client.ToEntity());
      }
      context.SaveChanges();
      logger.LogInformation($"Saved.");

      logger.LogInformation($"Removing {context.IdentityResources.Count()} IdentityResources.");
      foreach (var resource in context.IdentityResources)
      {
        context.IdentityResources.Remove(resource);
      }
      context.SaveChanges();
      logger.LogInformation($"Saved.");

      logger.LogInformation($"Adding {Config.GetIdentityResources().Count()} IdentityResources.");
      foreach (var resource in Config.GetIdentityResources())
      {
        context.IdentityResources.Add(resource.ToEntity());
      }

      context.SaveChanges();
      logger.LogInformation($"Saved.");

      logger.LogInformation($"Removing {context.ApiScopes.Count()} ApiScopes.");
      foreach (var resource in context.ApiScopes)
      {
        context.ApiScopes.Remove(resource);
      }
      context.SaveChanges();
      logger.LogInformation($"Saved.");

      logger.LogInformation($"Adding {context.ApiScopes.Count()} ApiScopes.");
      foreach (var resource in Config.GetApiScopes())
      {
        context.ApiScopes.Add(resource.ToEntity());
      }
      context.SaveChanges();
      logger.LogInformation($"Saved.");


      logger.LogInformation($"Removing {context.ApiScopes.Count()} ApiResources.");
      foreach (var resource in context.ApiResources)
      {
        context.ApiResources.Remove(resource);
      }
      context.SaveChanges();
      logger.LogInformation($"Saved.");

      logger.LogInformation($"Adding {Config.GetApis().Count()} ApiResources.");
      foreach (var resource in Config.GetApis())
      {
        context.ApiResources.Add(resource.ToEntity());
      }
      context.SaveChanges();
      logger.LogInformation($"Saved.");
    }


    public static void EnsureIdentityServerDatabaseCopy(IApplicationBuilder app)
    {
      using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
      {
        var logger = app.ApplicationServices.GetRequiredService<ILogger<DbInitialize>>();
        logger.LogInformation("Try migrating PersistedGrantDbContext");
        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
        logger.LogInformation("Try migrating ConfigurationDbContext");
        serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();

        var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

        logger.LogInformation($"Removing {context.Clients.Count()} Clients.");
        foreach (var client in context.Clients)
        {
          context.Clients.Remove(client);
        }
        context.SaveChanges();
        logger.LogInformation($"Saved.");

        logger.LogInformation($"Adding {Config.GetClients().Count()} clients.");
        foreach (var client in Config.GetClients())
        {
          context.Clients.Add(client.ToEntity());
        }
        context.SaveChanges();
        logger.LogInformation($"Saved.");

        logger.LogInformation($"Removing {context.IdentityResources.Count()} IdentityResources.");
        foreach (var resource in context.IdentityResources)
        {
          context.IdentityResources.Remove(resource);
        }

        logger.LogInformation($"Adding {Config.GetIdentityResources().Count()} IdentityResources.");
        foreach (var resource in Config.GetIdentityResources())
        {
          context.IdentityResources.Add(resource.ToEntity());
        }

        context.SaveChanges();
        logger.LogInformation($"Saved.");


        logger.LogInformation($"Removing {context.ApiScopes.Count()} ApiScopes.");
        foreach (var resource in context.ApiScopes)
        {
          context.ApiScopes.Add(resource);
        }
        context.SaveChanges();
        logger.LogInformation($"Saved.");

        logger.LogInformation($"Adding {context.ApiScopes.Count()} ApiScopes.");
        foreach (var resource in Config.GetApiScopes())
        {
          context.ApiScopes.Add(resource.ToEntity());
        }
        context.SaveChanges();
        logger.LogInformation($"Saved.");


        logger.LogInformation($"Removing {context.ApiScopes.Count()} ApiResources.");
        foreach (var resource in context.ApiResources)
        {
          context.ApiResources.Add(resource);
        }
        context.SaveChanges();
        logger.LogInformation($"Saved.");

        logger.LogInformation($"Adding {Config.GetApis().Count()} ApiResources.");
        foreach (var resource in Config.GetApis())
        {
          context.ApiResources.Add(resource.ToEntity());
        }
        context.SaveChanges();
        logger.LogInformation($"Saved.");
      }
    }
  }
}
