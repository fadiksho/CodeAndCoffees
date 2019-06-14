using MyBlog.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyBlog.Peristence.Data
{
  public static class DbInitialize
  {
    public static void SeedDb(BlogContext context, string url)
    {
      if (context.Blogs.Any())
        return;
      // Implement Init data for staging and development
      var blogs = new List<BlogTable>
      {
        new BlogTable
        {
          Title = "First Blog Seed",
          Slug = $"{DateTime.Now.Year}/{DateTime.Now.Month.ToString("D2")}/{DateTime.Now.Day.ToString("D2")}/First-Blog-Seed",
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
          Slug = $"{DateTime.Now.Year}/{DateTime.Now.Month.ToString("D2")}/{DateTime.Now.Day.ToString("D2")}/Second-Blog-Seed",
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
