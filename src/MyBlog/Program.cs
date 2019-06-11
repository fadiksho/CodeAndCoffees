using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyBlog.Peristence.Data;
using MyBlog.Services;

namespace MyBlog
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var host = CreateWebHostBuilder(args).Build();

      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<BlogContext>();
        context.Database.Migrate();
        var config = host.Services.GetRequiredService<IConfiguration>().Get<AppSettings>();
        var env = host.Services.GetRequiredService<IHostingEnvironment>();

        try
        {
          DbInitialize.Initialize(services, config.Token.AdminPassword).Wait();
          if (!(env.IsProduction() || env.IsStaging()))
          {
            DbInitialize.SeedDb(context, config.WebSiteHosting.Url);
          }
        }
        catch (Exception ex)
        {
          var logger = services.GetRequiredService<ILogger<Program>>();
          logger.LogError(ex, "An error occurred while seeding the database.");
        }
      }
      host.Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
  }
}
