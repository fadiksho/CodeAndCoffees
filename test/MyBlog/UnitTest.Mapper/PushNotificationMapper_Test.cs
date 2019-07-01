using AutoMapper;
using MyBlog.DTO;
using MyBlog.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using WebPush;
using Xunit;
namespace UnitTest.Mapper
{
  public class PushNotificationMapper_Test
  {
    private readonly IMapper mapper;
    public PushNotificationMapper_Test()
    {
      var mapperConfig = new MapperConfig();
      this.mapper = mapperConfig.GetIMapper();
    }

    [Fact]
    public void ConvertPushNotificationDtoToTable()
    {
      // arrange
      var pushNotificationDto = new PushNotificationSubscriptionDto()
      {
        AuthSecret = "auth secret",
        EndPoint = "end point",
        Key = "key"
      };
      var pushNotificationTable = new PushNotificationSubscriptionTable()
      {
        Id = 10,
        AuthSecret = "auth secret",
        EndPoint = "end point",
        Key = "key"
      };
      // act
      var pushNotification = mapper.Map<PushNotificationSubscriptionTable>(pushNotificationDto);
      // assert
      Assert.True(pushNotificationTable.IsPushNontificationTableEqual(pushNotification));
    }

    [Fact]
    public void ConvertPushNoticationTableToModel()
    {
      // arrange
      var pushSubscriptionTable = new PushNotificationSubscriptionTable()
      {
        Id = 10,
        AuthSecret = "auth secret",
        EndPoint = "end point",
        Key = "key"
      };
      var pushSubscriptionModel = new PushSubscription("end point", "key", "auth secret");
      // act
      var pushSubscription = mapper.Map<PushSubscription>(pushSubscriptionTable);
      // assert
      Assert.True(pushSubscription.IsPushSubscriptionEqual(pushSubscriptionModel));
    }
  }
}
