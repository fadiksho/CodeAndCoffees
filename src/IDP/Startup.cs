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
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using IdentityServer4.EntityFramework.DbContexts;
using System.Linq;

namespace IDP
{
  public class Startup
  {
    private readonly ILogger<Startup> logger;
    public IConfiguration Configuration { get; }
    public IHostingEnvironment Environment { get; }

    public Startup(IConfiguration configuration, IHostingEnvironment environment, ILogger<Startup> logger)
    {
      this.logger = logger;
      Configuration = configuration;
      Environment = environment;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      var appSetting = Configuration.Get<AppSettings>();
      var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
      logger.LogInformation($"Environmanet {Environment.EnvironmentName}");
      services.Configure<AppSettings>(Configuration);

      services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(appSetting.ConnectionStrings.DefaultConnection));

      services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

      //var signinCertificatePath = Path.Combine(Environment.ContentRootPath, "Credentials", "token_signing.pfx");
      //if (File.Exists(signinCertificatePath))
      //  logger.LogError($"Path found: {signinCertificatePath}");

      //var validationCertificatePath = Path.Combine(Environment.ContentRootPath, "Credentials", "token_validation.pfx");
      //if (File.Exists(validationCertificatePath))
      //  logger.LogError($"Path found: {validationCertificatePath}");

      //var signinCertificate = new X509Certificate2(signinCertificatePath, "IDon'tThinkThatThisWillWorkFromThe1Time", X509KeyStorageFlags.DefaultKeySet);

      //var validationCertificate = new X509Certificate2(validationCertificatePath, "IDon'tThinkThatThisWillWorkFromThe1Time",X509KeyStorageFlags.DefaultKeySet);
      //if (signinCertificate == null || validationCertificate == null)
      //{
      //  logger.LogError("Testing Failled Successfully");
      //}
      //var certPath = Path.Combine(Environment.ContentRootPath, "Credentials", "auth.codeandcoffees.crt");
      //var privateKey = Path.Combine(Environment.ContentRootPath, "Credentials", "private.auth.codeandcoffees.pem");
      //var cert = X509Helper.GetCertifcateWithPrivateKey(certPath, privateKey);
      
      var builder = services.AddIdentityServer(options =>
      {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
      });

      builder.AddConfigurationStore(options =>
      {
        options.ConfigureDbContext = b =>
          b.UseSqlServer(appSetting.ConnectionStrings.DefaultConnection,
            sql => sql.MigrationsAssembly(migrationsAssembly));
      })
        .AddOperationalStore(options =>
        {
          options.ConfigureDbContext = b =>
            b.UseSqlServer(appSetting.ConnectionStrings.DefaultConnection,
              sql => sql.MigrationsAssembly(migrationsAssembly));

          // this enables automatic token cleanup. this is optional.
          options.EnableTokenCleanup = true;
        })
        .AddAspNetIdentity<ApplicationUser>()
        .AddSigningCredential(new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "Certificates", "auth.codeandcoffees.com.pfx"),
                  "IForgetMyPassword-3Times!",
                  X509KeyStorageFlags.MachineKeySet));

      services.AddAuthentication();
      services.AddMvc()
        .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      //DbInitialize.EnsureIdentityServerDatabase(app);
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else
      {
        app.UseHttpsRedirection();
        app.UseHsts();
      }

      app.UseStaticFiles(new StaticFileOptions
      {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), @".well-known")),
        RequestPath = new PathString("/.well-known"),
        ServeUnknownFileTypes = true // serve extensionless file
      });
      app.UseStaticFiles(new StaticFileOptions
      {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "Certificates"))
      });
      app.UseStaticFiles();
      app.UseIdentityServer();
      app.UseMvcWithDefaultRoute();
    }
  }
}
