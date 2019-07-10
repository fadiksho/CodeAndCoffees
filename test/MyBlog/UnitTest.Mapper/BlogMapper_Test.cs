using AutoMapper;
using MyBlog.DTO;
using MyBlog.Entity;
using MyBlog.Model;
using System;
using System.Collections.Generic;
using UnitTest.Helpers;
using Xunit;

namespace UnitTest.Mapper
{
  public class BlogMapper_Test
  {
    private readonly IMapper mapper;
    public BlogMapper_Test()
    {
      var mapperConfig = new MapperConfig();
      this.mapper = mapperConfig.GetIMapper();
    }
    [Fact]
    public void ConvertBlogTableToModel()
    {
      // arrange
      var blogEntity = new BlogTable()
      {
        Id = 10,
        Title = "test convert",
        Slug = "2017/12/10/test-convert",
        Description = "test description",
        Body = "test body",
        PublishedDate = new DateTime(2017, 12, 10, 0, 0, 0),
        IsPublished = true,
        Tags = "C#, Angular"
      };
      var blogModel = new Blog()
      {
        Id = 10,
        Title = "test convert",
        Slug = "2017/12/10/test-convert",
        Description = "test description",
        Body = "test body",
        PublishedDate = new DateTime(2017, 12, 10, 0, 0, 0),
        IsPublished = true,
        Tags = new List<string>()
        {
          "C#",
          "Angular"
        }
      };
      // act
      var blog = mapper.Map<Blog>(blogEntity);
      // assert
      Assert.True(blogModel.IsBlogEqual(blog));
    }

    [Fact]
    public void ConvertBlogForCreatingDtoToTable()
    {
      // arrange
      var blogForCreatingDto = new BlogForCreatingDto()
      {
        Title = "test convert",
        Description = "test description",
        Body = "test body",
        PublishedDate = new DateTime(2017, 12, 10, 0, 0, 0),
        IsPublished = true,
        Tags = new List<string>()
        {
          "C#",
          "Angular"
        }
      };
      var blogTable = new BlogTable()
      {
        Id = 10,
        Title = "test convert",
        Slug = "test-convert",
        Description = "test description",
        Body = "test body",
        PublishedDate = new DateTime(2017, 12, 10, 0, 0, 0),
        IsPublished = true,
        Tags = "C#,Angular"
      };
      // act
      var blog = mapper.Map<BlogTable>(blogForCreatingDto);
      // assert
      Assert.True(blogTable.IsBlogTableEqualAfterConvertFromBlogForCreatingDto(blog));
    }

    [Fact]
    public void ConvertBlogForUpdatingDtoToTable()
    {
      // arrange
      var blogForCreatingDto = new BlogForUpdatingDto()
      {
        Title = "test convert",
        Description = "test description",
        Body = "test body",
        PublishedDate = new DateTime(2017, 12, 10, 0, 0, 0),
        IsPublished = true,
        Tags = new List<string>()
        {
          "C#",
          "Angular"
        }
      };
      var blogTable = new BlogTable()
      {
        Id = 10,
        Title = "test convert",
        Slug = "test-convert",
        Description = "test description",
        Body = "test body",
        PublishedDate = new DateTime(2017, 12, 10, 0, 0, 0),
        IsPublished = true,
        Tags = "C#,Angular"
      };
      // act
      var blog = mapper.Map<BlogTable>(blogForCreatingDto);
      // assert
      Assert.True(blogTable.IsBlogTableEqualAfterConvertFromBlogForUpdatingDto(blog));
    }
  }
}
