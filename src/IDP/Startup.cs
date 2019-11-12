using IDP.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using IDP.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using IDP.Models;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System;

namespace IDP
{
  public class Startup
  {
    public IConfiguration Configuration { get; }
    public IHostingEnvironment Environment { get; }

    public Startup(IConfiguration configuration, IHostingEnvironment environment)
    {
      Configuration = configuration;
      Environment = environment;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      var appSetting = Configuration.Get<AppSettings>();
      var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

      services.Configure<AppSettings>(Configuration);

      services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(appSetting.ConnectionStrings.DefaultConnection));

      services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

      services.AddMvc()
        .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);

      var builder = services.AddIdentityServer(options =>
      {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
      })
      .AddAspNetIdentity<ApplicationUser>();

      if (Environment.IsDevelopment())
      {
        builder
          .AddInMemoryIdentityResources(Config.GetIdentityResources())
          .AddInMemoryClients(Config.GetClients())
          .AddInMemoryApiResources(Config.GetApis())
          .AddDeveloperSigningCredential();
      }
      else
      {
        builder
        // config data from DB (clients, resources)
        .AddConfigurationStore(options =>
        {
          options.ConfigureDbContext = b =>
            b.UseSqlServer(appSetting.ConnectionStrings.DefaultConnection,
              sql => sql.MigrationsAssembly(migrationsAssembly));
        })
        // operational data from DB (codes, tokens, consents)
        .AddOperationalStore(options =>
        {
          options.ConfigureDbContext = b =>
            b.UseSqlServer(appSetting.ConnectionStrings.DefaultConnection,
              sql => sql.MigrationsAssembly(migrationsAssembly));

          // this enables automatic token cleanup. this is optional.
          options.EnableTokenCleanup = true;
        })
        .AddAspNetIdentity<ApplicationUser>();
        throw new Exception("configure key to signin the token!");
      }

      services.AddAuthentication();
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
