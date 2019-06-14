using IDP.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using IDP.Peristence.Data;
using Microsoft.EntityFrameworkCore;
using IDP.Models;
using Microsoft.AspNetCore.Identity;

namespace IDP
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      var appSetting = Configuration.Get<AppSettings>();

      services.Configure<AppSettings>(Configuration);

      services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(appSetting.ConnectionStrings.DefaultConnection));

      services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

      var builder = services.AddIdentityServer(options =>
      {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
      })
      // config data from DB (clients, resources)
      .AddConfigurationStore(options =>
      {
        options.ConfigureDbContext = b =>
          b.UseSqlServer(appSetting.ConnectionStrings.DefaultConnection);
      })
      // operational data from DB (codes, tokens, consents)
      .AddOperationalStore(options =>
      {
        options.ConfigureDbContext = b =>
          b.UseSqlServer(appSetting.ConnectionStrings.DefaultConnection);

        // this enables automatic token cleanup. this is optional.
        options.EnableTokenCleanup = true;
      })
      .AddAspNetIdentity<ApplicationUser>();

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      app.UseStaticFiles();
      app.UseIdentityServer();
      app.UseMvcWithDefaultRoute();
    }
  }
}
