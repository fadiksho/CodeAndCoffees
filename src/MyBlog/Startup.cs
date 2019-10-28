using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MyBlog.Persistence;
using MyBlog.Persistence.Data;
using MyBlog.Repository.Data;
using MyBlog.Services;
using AutoMapper;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Westwind.AspNetCore.LiveReload;

namespace MyBlog
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
      services.AddDbContext<BlogContext>(options =>
          options.UseSqlServer(appSetting.ConnectionStrings.DefaultConnection));

      services.AddScoped<IUnitOfWork, UnitOfWork>();

      services.AddSingleton<IFileHelper, FileHelper>();
      services.AddSingleton<IURLHelper, URLHelper>();

      services.AddLiveReload(config =>
      {
        // optional - use config instead
        // config.LiveReloadEnabled = true;
        // config.FolderToMonitor = Path.GetFullname(Path.Combine(Env.ContentRootPath,"..")) ;
      });

      services.AddAutoMapper(typeof(Startup));

      services.AddCors(o => o.AddPolicy("EnableCors", builder =>
      {
        builder
        .SetIsOriginAllowedToAllowWildcardSubdomains()
          .WithOrigins(
            "https://*.codeandcoffees.com",
            "https://staging.blogmanager.codeandcoffees.com",
            "http://localhost:4200"
          )
          .AllowAnyHeader()
          .AllowAnyMethod();
      }));

      services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
          options.RequireHttpsMetadata = appSetting.WebSiteHosting.RequirdHttps;
          options.Authority = appSetting.JwtBearer.Authority;
          options.Audience = appSetting.JwtBearer.Audience;
        });

      services.AddMvc()
        .AddJsonOptions(opt =>
        {
          opt.SerializerSettings.ContractResolver =
            new CamelCasePropertyNamesContractResolver();
        })
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");

      if (env.IsDevelopment())
      {
        // ToDo: check if there is a bug in this package when we hit statuscode controller
        // it return 500 error instead of the specifid view.
        app.UseLiveReload();
        // app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
        // {
        //   ProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp"),
        //   // HotModuleReplacement = true
        // });
        app.UseDeveloperExceptionPage();
      }
      else if (env.IsProduction() || env.IsStaging())
      {
        app.UseExceptionHandler(errorApp =>
        {
          errorApp.Run(async context =>
          {
            context.Response.StatusCode = 500;
          });
        });
        app.UseHsts();
        app.UseHttpsRedirection();
      }

      // Disable html view response on api request
      app.Use(async (ctx, next) =>
      {
        if (ctx.Request.Path.StartsWithSegments("/api", System.StringComparison.OrdinalIgnoreCase))
        {
          var statusCodeFeature = ctx.Features.Get<IStatusCodePagesFeature>();

          if (statusCodeFeature != null && statusCodeFeature.Enabled)
            statusCodeFeature.Enabled = false;
        }

        await next();
      });

      app.UseCors("EnableCors");
      app.UseStaticFiles(new StaticFileOptions
      {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @".well-known")),
        RequestPath = new PathString("/.well-known"),
        ServeUnknownFileTypes = true // serve extensionless file
      });
      app.UseStaticFiles(new StaticFileOptions
      {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles")),
        RequestPath = "/UploadedFiles"
      });
      app.UseStaticFiles();

      app.UseAuthentication();
      app.UseMvc(routes =>
      {
        routes.MapRoute("Blog", "post/{*slug}",
          defaults: new { controller = "Blog", action = "Detail" });
        routes.MapRoute("BlogPage", "blog/page/{pageNumber:int}",
          defaults: new { controller = "Blog", action = "Pagination" });
        routes.MapRoute("Defaults", "{controller=Blog}/{action=Index}/{id?}");
      });
    }
  }
}


