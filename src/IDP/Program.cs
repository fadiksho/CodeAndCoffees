using IDP.Persistence.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;
using System;

namespace IDP
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
      try
      {
        var host = CreateWebHostBuilder(args).Build();
        using (var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
          DbInitialize.EnsureSeedData(scope.ServiceProvider);
        }
        host.Run();
      }
      catch (Exception ex)
      {
        //NLog: catch setup errors
        logger.Error(ex, "Stopped program because of exception");
        throw;
      }
      finally
      {
        NLog.LogManager.Shutdown();
      }
    }
    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureLogging(logging =>
            {
              logging.ClearProviders();
              logging.AddConsole();
              logging.AddDebug();
            })
            .UseNLog();
  }
}
