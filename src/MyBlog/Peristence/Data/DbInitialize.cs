using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.Authorization;
using MyBlog.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Peristence.Data
{
  public static class DbInitialize
  {
    public static async Task Initialize(IServiceProvider serviceProvider, string adminPW)
    {
      using (var context = new BlogContext(
        serviceProvider.GetRequiredService<DbContextOptions<BlogContext>>()))
      {
        var adminID = await EnsureUser(serviceProvider, "admin@codeandcoffees.com", adminPW);
        await EnsureRole(serviceProvider, adminID, Constants.Admin);
      }
    }

    private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                            string userName, string password)
    {
      var userManager = serviceProvider.GetService<UserManager<User>>();

      var user = await userManager.FindByNameAsync(userName);
      if (user == null)
      {
        user = new User { UserName = userName };
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
          throw new Exception("Seeding data faild!");
        }
      }

      return user.Id;
    }
    private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                        string uid, string role)
    {
      IdentityResult IR = null;
      var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

      if (roleManager == null)
      {
        throw new Exception("RoleManager is null");
      }

      if (!await roleManager.RoleExistsAsync(role))
      {
        IR = await roleManager.CreateAsync(new IdentityRole(role));
      }

      var userManager = serviceProvider.GetService<UserManager<User>>();

      var user = await userManager.FindByIdAsync(uid);

      IR = await userManager.AddToRoleAsync(user, role);

      return IR;
    }
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
          Slug = "First-Blog-Seed",
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
          Slug = "Second-Blog-Seed",
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
