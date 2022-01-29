using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MyBlog.Persistence.Data;
using NLog.Web;
using System;

namespace MyBlog
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
      try
      {
        var host = CreateWebHostBuilder(args).Build();
        
        DbSetup.EnsureMigrationAndSeeding(host, logger);

        host.Run();
      }
      catch (Exception exception)
      {
        //NLog: catch setup errors
        logger.Error(exception, "Stopped program because of exception");
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
