using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using MyBlog.Extensions;
using MyBlog.Persistence;
using MyBlog.Persistence.Data;
using MyBlog.Repository.Data;
using MyBlog.Services;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
        .AddJsonOptions(opt =>
        {
          opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

builder.Services.AddRazorPages();

builder.Services.AddSettingsConfiguration(builder.Configuration);

builder.Services
  .AddDatabaseContext<BlogContext>()
  .AddScoped<IUnitOfWork, UnitOfWork>()
  .AddSingleton<IFileHelper, FileHelper>()
  .AddSingleton<IURLHelper, URLHelper>()
  .AddSingleton<ILoginService, LoginService>()
  .AddAutoMapper(typeof(Program));

builder.Services
  .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
  .AddCookie(options =>
  {
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.LoginPath = "/admin/login/";
    //options.LogoutPath = "/logout/";
    options.Events.OnRedirectToLogin = async (cookieAuthenticationOptions) =>
    {
      cookieAuthenticationOptions.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
      await cookieAuthenticationOptions.Response.WriteAsync("Error");
    };
  });

var app = builder.Build();

app.UseCustomExceptionHandler();

app.UseHttpsRedirection();

if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"UploadedFiles")))
{
  Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"UploadedFiles"));
}
app.UseStaticFiles(new StaticFileOptions
{

  FileProvider = new PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), @"UploadedFiles")),
  RequestPath = new PathString("/UploadedFiles")
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Map("/admin", adminApp =>
{
  adminApp.UseEndpoints(adminEndPoint =>
  {
    adminEndPoint.MapFallbackToFile("/admin/", "/admin/index.html");
  });
});

app.UseEndpoints(endpoints =>
{
  endpoints.MapControllerRoute("default", "{controller=Blog}/{action=Index}/{id?}");
});


app.Run();
