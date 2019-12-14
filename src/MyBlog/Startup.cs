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
using System.Net;

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
      app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), subApp =>
      {
        subApp.UseExceptionHandler(builder =>
        {
          builder.Run(async context =>
          {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync("Error");
          });
        });
      });
      app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), subApp =>
      {
        subApp.UseExceptionHandler("/StatusCode/500");
        subApp.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
      });

      if (env.IsDevelopment())
      {
        // ToDo: check if there is a bug in this package when we hit statuscode controller
        // it return 500 error instead of the specifid view.
        app.UseDeveloperExceptionPage();
      }
      else if (env.IsProduction() || env.IsStaging())
      {
        app.UseHttpsRedirection();
        app.UseHsts();
      }
      
      app.UseCors("EnableCors");
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
            Path.Combine(Directory.GetCurrentDirectory(), @"UploadedFiles")),
        RequestPath = new PathString("/UploadedFiles")
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


