using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.Services;
using MyBlog.Settings;

namespace MyBlog.Extensions
{
  public static class ServiceCollectionExtensions
  {
    public static IServiceCollection AddDatabaseContext<T>(this IServiceCollection services)
            where T : DbContext
    {
      var options = services.GetOptions<PersistenceSettings>(nameof(PersistenceSettings));

      if (options.UseSqlLite)
      {
        services.addSqllite<T>(options.ConnectionStrings.SqlLite);
      }
      else if (options.UseSqlServer)
      {
        services.addSqlServer<T>(options.ConnectionStrings.SqlServer);
      }

      return services;
    }

    public static IServiceCollection AddSettingsConfiguration(this IServiceCollection services, IConfiguration config)
    {
      return services
        .Configure<LoginSettings>(config.GetSection(nameof(LoginSettings)))
        .Configure<PersistenceSettings>(config.GetSection(nameof(PersistenceSettings)))
        .Configure<BlogSettings>(config.GetSection(nameof(BlogSettings)))
        .Configure<CommentSettings>(config.GetSection(nameof(CommentSettings)))
        .Configure<PushNotificationSettings>(config.GetSection(nameof(PushNotificationSettings)));
    }


    #region Helpers
    private static T GetOptions<T>(this IServiceCollection services, string sectionName)
        where T : new()
    {
      using var serviceProvider = services.BuildServiceProvider();
      var configuration = serviceProvider.GetRequiredService<IConfiguration>();
      var section = configuration.GetSection(sectionName);
      var options = new T();
      section.Bind(options);

      return options;
    }

    private static IServiceCollection addSqlServer<T>(this IServiceCollection services, string connectionString)
          where T : DbContext
    {
      services.AddDbContext<T>(m => m.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(T).Assembly.FullName)));

      using var scope = services.BuildServiceProvider().CreateScope();
      var dbContext = scope.ServiceProvider.GetRequiredService<T>();
      dbContext.Database.Migrate();

      return services;
    }

    private static IServiceCollection addSqllite<T>(this IServiceCollection services, string connectionString)
        where T : DbContext
    {
      services.AddDbContext<T>(m => m.UseSqlite(connectionString, b => b.MigrationsAssembly(typeof(T).Assembly.FullName)));

      using var scope = services.BuildServiceProvider().CreateScope();
      var dbContext = scope.ServiceProvider.GetRequiredService<T>();
      dbContext.Database.Migrate();

      return services;
    }
    #endregion
  }
}

