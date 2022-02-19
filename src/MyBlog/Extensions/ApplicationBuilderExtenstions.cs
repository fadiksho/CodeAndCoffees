using System.Net;

namespace MyBlog.Extensions
{
  public static class ApplicationBuilderExtenstions
  {
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
    {
      builder.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), subApp =>
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

      builder.UseWhen(context =>
        //!context.Request.Path.StartsWithSegments("/api") && 
        !context.Request.Path.StartsWithSegments("/admin"), subApp =>
      {
        subApp.UseExceptionHandler("/StatusCode/500");
        subApp.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
      });

      return builder;
    }
  }
}
