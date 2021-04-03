using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyBlog.Entity;
using MyBlog.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.Persistence.Data
{
  public static class DbSetup
  {
    public static void EnsureMigrationAndSeeding(IWebHost host, Logger logger)
    {
      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<BlogContext>();
        var config = host.Services.GetRequiredService<IConfiguration>().Get<AppSettings>();
        var env = host.Services.GetRequiredService<IWebHostEnvironment>();

        logger.Debug("Apply Latest Migration.");
        try
        {
          EnsureLatestMigration(context);
        }
        catch (Exception ex)
        {
          logger.Error(ex, "An error occurred while applying migration on the database.");
          throw ex;
        }
        try
        {
          EnsureSeedDb(env, context, config);
        }
        catch (Exception ex)
        {
          logger.Error(ex, "An error occurred while seeding the database.");
          throw ex;
        }
      }
    }
    public static void EnsureLatestMigration(BlogContext context)
    {
      context.Database.Migrate();
    }
    public static void EnsureSeedDb(IWebHostEnvironment env, BlogContext context, AppSettings appSettings)
    {
      if (env.IsProduction() || env.IsStaging())
        return;

      if (context.Blogs.Any())
        return;

      // Implement Init data for staging and development
      var blogs = new List<BlogTable>
      {
        new BlogTable
        {
          Title = "First Blog Seed",
          Slug = $"First-Blog-Seed",
          Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
          Body = "<h1>Blog Body</h1><p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p><p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p>",
          KeyWords = "test key",
          IsPublished = true,
          Tags = "C#,C++,Angular,html",
          PublishedDate = DateTime.Now
        },
        new BlogTable
        {
          Title = "Second Blog Seed",
          Slug = $"Second-Blog-Seed",
          Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
          Body = "<h1>Blog Body</h1><p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p><p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.</p>",
          KeyWords = "test key",
          IsPublished = true,
          Tags = "C#,C++,Angular,html",
          PublishedDate = DateTime.Now
        }
      };
      context.AddRange(blogs);
      context.SaveChanges();
    }
  }
}
