using AutoMapper;
using MyBlog.Abstraction;
using MyBlog.Entity;
using MyBlog.Persistence.Data;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnitTest.Helpers;
using Xunit;

namespace UnitTest.Repository
{
  public class BlobRepository_Test
  {
    private readonly IMapper mapper;
    public BlobRepository_Test()
    {
      var mapperConfig = new MapperConfig();
      this.mapper = mapperConfig.GetIMapper();
    }

    [Fact]
    public async Task Add_Blob()
    {
      using (var factory = new BlogContextFactory())
      {
        bool isSaved = false;
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var blobTable = GetBlobTableInstance();
          // Act
          await unitOfWork.Blobs.AddBlobAsync(blobTable);
          isSaved = await unitOfWork.SaveAsync();
        }

        using (var context = factory.CreateBlogContext())
        {
          // Assert
          Assert.NotNull(context.Blobs.First());
          Assert.True(isSaved);
        }
      }
    }

    [Fact]
    public async Task Is_Blob_Exist_Should_be_True_When_Blob_Exist()
    {
      using (var factory = new BlogContextFactory())
      {
        var blobId = 0;
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var blobTable = GetBlobTableInstance();
          context.Add(blobTable);
          context.SaveChanges();
          blobId = blobTable.Id;

        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          var isExist = await unitOfWork.Blobs.BlobExistAsync(blobId);
          // Assert
          Assert.True(isExist);
        }
      }
    }

    [Fact]
    public async Task Is_Blob_Exist_Should_be_False_When_Blob_Is_Not_Exist()
    {
      using (var factory = new BlogContextFactory())
      {
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var blobTable = GetBlobTableInstance();
          context.Add(blobTable);
          context.SaveChanges();
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          var isExist = await unitOfWork.Blobs.BlobExistAsync(999);
          // Assert
          Assert.False(isExist);
        }
      }
    }

    [Fact]
    public async Task Get_Blob_Should_Not_Be_Null()
    {
      using (var factory = new BlogContextFactory())
      {
        var blobId = 0;
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var blobTable = GetBlobTableInstance();
          context.Add(blobTable);
          context.SaveChanges();
          blobId = blobTable.Id;
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          var blob = await unitOfWork.Blobs.GetBlobAsync(blobId);
          // Assert
          Assert.NotNull(blob);
        }
      }
    }

    [Fact]
    public async Task Get_Blob_Should_Throw_InvalidOperationException()
    {
      using (var factory = new BlogContextFactory())
      {
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var blobTable = GetBlobTableInstance();
          context.Add(blobTable);
          context.SaveChanges();
        }

        using (var context = factory.CreateBlogContext())
        {
          var unitOfWork = new UnitOfWork(context, mapper);
          // Act & Assert
          await Assert.ThrowsAsync<InvalidOperationException>(() => unitOfWork.Blobs.GetBlobAsync(999));
        }
      }
    }

    [Fact]
    public void Get_Blob_Page_Should_Contain_2_Blobs()
    {
      using (var factory = new BlogContextFactory())
      {
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var blobTable1 = GetBlobTableInstance();
          var blobTable2 = GetBlobTableInstance();
          context.Add(blobTable1);
          context.Add(blobTable2);
          context.SaveChanges();
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          var blobspage = unitOfWork.Blobs.GetBlobPage(new PaggingQuery());
          // Assert
          Assert.Equal(2, blobspage.TResult.Count());
          Assert.Equal(2, blobspage.TotalItems);
          Assert.Equal(1, blobspage.TotalPages);
          Assert.Equal(10, blobspage.PageSize);
        }
      }
    }

    [Fact]
    public void Get_Blob_Page_Should_Be_In_Descending_Order_By_CreatedDate()
    {
      using (var factory = new BlogContextFactory())
      {
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var blobTable1 = GetBlobTableInstance(name: "first", createdDate: "01/01/2019");
          var blobTable2 = GetBlobTableInstance(name: "second", createdDate: "02/01/2019");
          context.Add(blobTable1);
          context.Add(blobTable2);
          context.SaveChanges();
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          var blob = unitOfWork.Blobs.GetBlobPage(new PaggingQuery()).TResult.First();
          // Assert
          Assert.Equal("second", blob.Name);
        }
      }
    }

    [Fact]
    public async Task Delete_Blob()
    {
      using (var factory = new BlogContextFactory())
      {
        var blobId = 0;
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var blobTable = GetBlobTableInstance();
          context.Add(blobTable);
          context.SaveChanges();
          blobId = blobTable.Id;
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          await unitOfWork.Blobs.DeleteBlobAsync(blobId);
          var isBlobDeleted = await unitOfWork.SaveAsync();
          // Assert
          Assert.True(isBlobDeleted);
          Assert.Null(context.Blobs.FirstOrDefault());
        }
      }
    }

    [Fact]
    public async Task Delete_Blob_Should_Throw_InvalidOperationException()
    {
      using (var factory = new BlogContextFactory())
      {
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var blobTable = GetBlobTableInstance();
          context.Add(blobTable);
          context.SaveChanges();
        }

        using (var context = factory.CreateBlogContext())
        {
          var unitOfWork = new UnitOfWork(context, mapper);
          // Act && Assert
          await Assert.ThrowsAsync<InvalidOperationException>(() => unitOfWork.Blobs.DeleteBlobAsync(999));
        }
      }
    }

    [Fact]
    public async Task Get_Blob_Path_Should_Throw_InvalidOperationException()
    {
      using (var factory = new BlogContextFactory())
      {
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var blobTable = GetBlobTableInstance();
          context.Add(blobTable);
          context.SaveChanges();
        }

        using (var context = factory.CreateBlogContext())
        {
          var unitOfWork = new UnitOfWork(context, mapper);
          // Act && Assert
          await Assert.ThrowsAsync<InvalidOperationException>(() => unitOfWork.Blobs.GetBlobPathAsync(999));
        }
      }
    }
    private BlobTable GetBlobTableInstance(
      string fileName = "filename",
      string filePath = "filePath",
      string createdDate = "01/01/2019",
      long fileSize = 9999,
      string name = "name",
      string url = "url"
      )
    {
      return new BlobTable
      {
        Name = name,
        FileName = fileName,
        FilePath = filePath,
        CreatedDate = DateTime.ParseExact(createdDate, "MM/dd/yyyy", CultureInfo.InvariantCulture),
        FileSize = fileSize,
        URL = url
      };
    }
  }
}
