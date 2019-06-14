using IDP.Peristence.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace IDP
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var host = CreateWebHostBuilder(args).Build();
      using (var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
      {
        DbInitialize.EnsureSeedData(scope.ServiceProvider);
      }
      host.Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
  }
}
