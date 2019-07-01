using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyBlog.Peristence.Data;
using System;
using System.Data.Common;

namespace UnitTest.Repository
{
  public class BlogContextFactory : IDisposable
  {
    private DbConnection connection;

    private DbContextOptions<BlogContext> CreateOptions()
    {
      return new DbContextOptionsBuilder<BlogContext>()
        .UseSqlite(connection).Options;
    }

    public BlogContext CreateBlogContext()
    {
      if (connection == null)
      {
        connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = CreateOptions();
        using (var context = new BlogContext(options))
        {
          context.Database.EnsureCreated();
        }
      }

      return new BlogContext(CreateOptions());
    }
    public void Dispose()
    {
      if (connection != null)
      {
        connection.Dispose();
        connection = null;
      }
    }
  }
}
