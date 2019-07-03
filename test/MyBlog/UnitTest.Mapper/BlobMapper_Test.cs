using AutoMapper;
using MyBlog.Entity;
using MyBlog.Model;
using System;
using UnitTest.Helpers;
using Xunit;

namespace UnitTest.Mapper
{
  public class BlobMapper_Test
  {
    private readonly IMapper mapper;
    public BlobMapper_Test()
    {
      var mapperConfig = new MapperConfig();
      this.mapper = mapperConfig.GetIMapper();
    }

    [Fact]
    public void ConvertBlobTableToModel()
    {
      // arrange
      var blobEntity = new BlobTable()
      {
        Id = 10,
        CreatedDate = new DateTime(2017, 12, 10, 0, 0, 0),
        Name = "filename",
        FileName = "filename.ext",
        FilePath = "filepath",
        FileSize = 9999,
        URL = "url"
      };
      var blobModel = new Blob()
      {
        Id = 10,
        CreatedDate = new DateTime(2017, 12, 10, 0, 0, 0),
        Name = "filename",
        FileSize = 9999,
        URL = "url"
      };
      // act
      var blob = mapper.Map<Blob>(blobEntity);
      // assert
      Assert.True(blobModel.IsBlobEqual(blob));
    }
  }
}
