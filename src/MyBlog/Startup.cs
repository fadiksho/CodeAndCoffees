//using System.IO;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.FileProviders;
//using MyBlog.Persistence;
//using MyBlog.Persistence.Data;
//using MyBlog.Repository.Data;
//using MyBlog.Services;
//using System.Net;
//using System.Text.Json;
//using Microsoft.Extensions.Hosting;
//using MyBlog.Extensions;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.SpaServices.AngularCli;

//namespace MyBlog
//{
//  public class Startup
//  {
//    public Startup(IConfiguration configuration)
//    {
//      Configuration = configuration;
//    }

//    public IConfiguration Configuration { get; }

//    // This method gets called by the runtime. Use this method to add services to the container.
//    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
//    public void ConfigureServices(IServiceCollection services)
//    {
//      services.AddControllersWithViews()
//        .AddJsonOptions(opt =>
//        {
//          opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
//        });

//      services.AddRazorPages();

//      services.AddSettingsConfiguration(Configuration);

//      services.AddDatabaseContext<BlogContext>();

//      services.AddScoped<IUnitOfWork, UnitOfWork>();

//      services.AddSingleton<IFileHelper, FileHelper>();
//      services.AddSingleton<IURLHelper, URLHelper>();
//      services.AddSingleton<ILoginService, LoginService>();

//      services.AddAutoMapper(typeof(Startup));

//      services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//        .AddCookie(options =>
//        {
//          options.LoginPath = "/login/";
//          options.LogoutPath = "/logout/";
//          options.Events.OnRedirectToLogin = async (cookieAuthenticationOptions) =>
//          {
//            cookieAuthenticationOptions.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
//            await cookieAuthenticationOptions.Response.WriteAsync("Error");
//          };
//        });

//      // In production, the Angular files will be served from this directory
//      services.AddSpaStaticFiles(configuration =>
//      {
//        configuration.RootPath = "BlogManager/dist";
//      });
//    }

//    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//    {
//      app.UseCustomExceptionHandler();

//      app.UseRouting();

//      app.UseStaticFiles(new StaticFileOptions
//      {
//        FileProvider = new PhysicalFileProvider(
//            Path.Combine(Directory.GetCurrentDirectory(), @"UploadedFiles")),
//        RequestPath = new PathString("/UploadedFiles")
//      });
//      app.UseStaticFiles();
      
//      app.UseAuthentication();
//      app.UseAuthorization();

//      app.Map("/admin", adminApp =>
//      {
//        adminApp.UseSpa(spa =>
//        {
//          spa.Options.SourcePath = "BlogManager";

//          if (env.IsDevelopment())
//          {
//            spa.UseAngularCliServer(npmScript: "start");
//          }
//        });
//      });

//      app.UseEndpoints(endpoints =>
//      {
//        endpoints.MapControllerRoute("default", "{controller=Blog}/{action=Index}/{id?}");
//      });
//    }
//  }
//}
