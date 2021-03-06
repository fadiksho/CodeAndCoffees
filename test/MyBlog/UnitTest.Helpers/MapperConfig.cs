﻿using AutoMapper;
using MyBlog.MappingProfiles;

namespace UnitTest.Helpers
{
  public class MapperConfig
  {
    public IMapper GetIMapper()
    {
      var config = new MapperConfiguration(cfg =>
      {
        cfg.AddProfile<BlogProfile>();
        cfg.AddProfile<PushNotificationProfile>();
        cfg.AddProfile<BlobProfile>();
      });

      return config.CreateMapper();
    }
  }
}
