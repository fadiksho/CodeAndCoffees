using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MyBlog.Peristence;
using MyBlog.Peristence.Data;
using MyBlog.Repository;
using MyBlog.Repository.Data;
using MyBlog.Services;
using AutoMapper;
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
      services.AddScoped<IFileHelper, FileHelper>();

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
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else if (env.IsProduction() || env.IsStaging())
      {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
        app.UseHttpsRedirection();
      }

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
      app.UseMvc();
    }
  }
}


