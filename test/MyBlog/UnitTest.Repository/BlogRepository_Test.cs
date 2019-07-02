using AutoMapper;
using MyBlog.DTO;
using MyBlog.Entity;
using MyBlog.Model;
using MyBlog.Peristence.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnitTest.Helpers;
using Xunit;

namespace UnitTest.Repository
{
  public class BlogRepository_Test
  {
    private readonly IMapper mapper;
    public BlogRepository_Test()
    {
      var mapperConfig = new MapperConfig();
      this.mapper = mapperConfig.GetIMapper();
    }

    [Fact]
    public async Task Adding_Blog_Should_Persiste()
    {
      using (var factory = new BlogContextFactory())
      {
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var blogForCreating = GetBlogForCreatingDtoInstance();
          // Act
          await unitOfWork.Blogs.AddBlog(blogForCreating);
          await unitOfWork.SaveAsync();
        }

        using (var context = factory.CreateBlogContext())
        {
          // Assert
          Assert.NotNull(context.Blogs.First());
        }
      }
    }

    [Fact]
    public async Task Updating_Blog_Should_Persiste()
    {
      using (var factory = new BlogContextFactory())
      {
        int blogId = 0;
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var blogTable = GetBlogTableInstance();
          context.Add(blogTable);
          context.SaveChanges();

          blogId = blogTable.Id;
        }

        using (var context = factory.CreateBlogContext())
        {
          var unitOfWork = new UnitOfWork(context, mapper);
          var blogForUpdatingDto = GetBlogForUpdatingDtoInstance(
            title: "updated",
            tags: new List<string> { "test" });
          // Act
          await unitOfWork.Blogs.UpdateBlogAsync(blogId, blogForUpdatingDto);
          var isBlogUpdated = await unitOfWork.SaveAsync();
          // Assert
          Assert.True(isBlogUpdated);
        }
      }
    }

    [Fact]
    public async Task Is_Blog_Exist_Should_be_False_When_Blog_Is_Not_Published()
    {
      using (var factory = new BlogContextFactory())
      {
        int blogId = 0;
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var blogTable = GetBlogTableInstance(isPublished: false);
          context.Add(blogTable);
          context.SaveChanges();

          blogId = blogTable.Id;
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          var isBlogExist = await unitOfWork.Blogs.BlogExistAsync(blogId, true);
          // Assert
          Assert.False(isBlogExist);
        }
      }
    }

    [Fact]
    public async Task Is_Blog_Exist_Should_be_True_When_Blog_Is_Published()
    {
      using (var factory = new BlogContextFactory())
      {
        int blogId = 0;
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var blogTable = GetBlogTableInstance();
          context.Add(blogTable);
          context.SaveChanges();

          blogId = blogTable.Id;
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          var isBlogExist = await unitOfWork.Blogs.BlogExistAsync(blogId, true);
          // Assert
          Assert.True(isBlogExist);
        }
      }
    }

    [Fact]
    public async Task Delete_Blog_Should_Persiste()
    {
      using (var factory = new BlogContextFactory())
      {
        int blogId = 0;
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var blogTable = GetBlogTableInstance();
          context.Add(blogTable);
          context.SaveChanges();

          blogId = blogTable.Id;
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          await unitOfWork.Blogs.DeleteBlogAsync(blogId);
          var isBlogDeleted = await unitOfWork.SaveAsync();
          // Assert
          Assert.True(isBlogDeleted);
          Assert.Null(context.Blogs.FirstOrDefault());
        }
      }
    }

    [Fact]
    public async Task Get_Blog_By_Id_Should_Not_Be_Null()
    {
      using (var factory = new BlogContextFactory())
      {
        int blogId = 0;
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var blogTable = GetBlogTableInstance();
          context.Add(blogTable);
          context.SaveChanges();

          blogId = blogTable.Id;
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          var blog = await unitOfWork.Blogs.GetBlogAsync(blogId);
          // Assert
          Assert.NotNull(blog);
        }
      }
    }

    [Theory]
    [InlineData("2017/12/10/test-convert")]
    [InlineData("2017/12/10/TEST-CONVERT")]
    public async Task Get_Blog_By_Slug_Should_Not_Be_Null(string slug)
    {
      using (var factory = new BlogContextFactory())
      {
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var blogTable = GetBlogTableInstance(slug: slug);
          context.Add(blogTable);
          context.SaveChanges();
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          var blog = await unitOfWork.Blogs.GetBlogAsync(slug);
          // Assert
          Assert.NotNull(blog);
        }
      }
    }

    [Fact]
    public void Get_Blog_Page_Should_Contain_2_Blogs()
    {
      using (var factory = new BlogContextFactory())
      {
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var blogTable1 = GetBlogTableInstance();
          var blogTable2 = GetBlogTableInstance();

          context.Add(blogTable1);
          context.Add(blogTable2);
          context.SaveChanges();
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          var blogsPage = unitOfWork.Blogs.GetBlogsPage(new BlogQuery());
          // Assert
          Assert.Equal(2, blogsPage.TResult.Count());
          Assert.Equal(2, blogsPage.TotalItems);
          Assert.Equal(1, blogsPage.TotalPages);
          Assert.Equal(10, blogsPage.PageSize);
        }
      }
    }

    [Fact]
    public void Get_Blog_Page_Should_Be_In_Descending_Order_By_UpdatedDate()
    {
      using (var factory = new BlogContextFactory())
      {
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var blogTable1 = GetBlogTableInstance(title: "first", publishDate: "01/01/2019");
          var blogTable2 = GetBlogTableInstance(title: "second", publishDate: "02/01/2019");

          context.Add(blogTable1);
          context.Add(blogTable2);
          context.SaveChanges();
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          var blog = unitOfWork.Blogs.GetBlogsPage(new BlogQuery()).TResult.First();
          // Assert
          Assert.Equal("second", blog.Title);
        }
      }
    }

    private BlogForCreatingDto GetBlogForCreatingDtoInstance(
    string title = "test convert",
      string description = "test description",
      string body = "test body",
      string publishDate = "01/01/2019",
      bool isPublished = true,
      List<string> tags = null)
    {
      if (tags == null)
        tags = new List<string> { "C#", "Angular" };

      return new BlogForCreatingDto
      {
        Title = title,
        Description = description,
        Body = body,
        PublishedDate = DateTime.ParseExact(publishDate, "MM/dd/yyyy", CultureInfo.InvariantCulture),
        IsPublished = isPublished,
        Tags = tags
      };
    }

    private BlogForUpdatingDto GetBlogForUpdatingDtoInstance(
      string title = "test convert",
      string description = "test description",
      string body = "test body",
      string publishDate = "01/01/2019",
      bool isPublished = true,
      List<string> tags = null)
    {
      if (tags == null)
        tags = new List<string> { "C#", "Angular" };

      return new BlogForUpdatingDto
      {
        Title = title,
        Description = description,
        Body = body,
        PublishedDate = DateTime.ParseExact(publishDate, "MM/dd/yyyy", CultureInfo.InvariantCulture),
        IsPublished = isPublished,
        Tags = tags
      };
    }

    private BlogTable GetBlogTableInstance(
      string title = "test convert",
      string slug = "2017/12/10/test-convert",
      string description = "test description",
      string body = "test body",
      string publishDate = "01/01/2019",
      bool isPublished = true,
      string tags = "C#,Angular")
    {
      return new BlogTable
      {
        Title = title,
        Slug = slug,
        Description = description,
        Body = body,
        PublishedDate = DateTime.ParseExact(publishDate, "MM/dd/yyyy", CultureInfo.InvariantCulture),
        IsPublished = isPublished,
        Tags = tags
      };
    }
  }
}
