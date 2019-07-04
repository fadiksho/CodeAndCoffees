using AutoMapper;
using MyBlog.DTO;
using MyBlog.Entity;
using MyBlog.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Helpers;
using Xunit;

namespace UnitTest.Repository
{
  public class SubscriberRepository_Test
  {
    private readonly IMapper mapper;
    public SubscriberRepository_Test()
    {
      var mapperConfig = new MapperConfig();
      this.mapper = mapperConfig.GetIMapper();
    }

    [Fact]
    public async Task Add_PushNotificationSubscriber()
    {
      using (var factory = new BlogContextFactory())
      {
        bool isSaved = false;
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var subscriptionDto = GetPushNotificationSubscriptionDtoInstance();
          // Act
          await unitOfWork.Subscribers.AddPushNotificationSubscriptionAsync(subscriptionDto);
          isSaved = await unitOfWork.SaveAsync();
        }

        using (var context = factory.CreateBlogContext())
        {
          // Assert
          Assert.NotNull(context.PushNotificationSubscriptions.First());
          Assert.True(isSaved);
        }
      }
    }

    [Fact]
    public async Task Get_All_PushNotificationSubscribers_Should_Not_Be_Null()
    {
      using (var factory = new BlogContextFactory())
      {
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var subscriptionTable1 = GetPushNotificationSubscriptionTableInstance();
          var subscriptionTable2 = GetPushNotificationSubscriptionTableInstance();
          context.Add(subscriptionTable1);
          context.Add(subscriptionTable2);
          context.SaveChanges();
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          var subscribers = await unitOfWork.Subscribers.GetPushNotificationSubscriptionAsync();
          
          // Assert
          Assert.Equal(2, subscribers.Count());
        }
      }
    }

    [Theory]
    [InlineData("endPoint")]
    public async Task Delete_PushNotification(string endPoint)
    {
      using (var factory = new BlogContextFactory())
      {
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var subscriptionTable1 = GetPushNotificationSubscriptionTableInstance(endPoint: endPoint);
          context.Add(subscriptionTable1);
          context.SaveChanges();
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          await unitOfWork.Subscribers.RemovePushNotificationSubscriptionAsync(endPoint);
          var isDeleted = await unitOfWork.SaveAsync();
          var subscriber = context.PushNotificationSubscriptions.FirstOrDefault();

          // Assert
          Assert.True(isDeleted);
          Assert.Null(subscriber);
        }
      }
    }

    [Theory]
    [InlineData("endPoint")]
    public async Task Get_PushNotification_Should_Not_Be_Null(string endPoint)
    {
      using (var factory = new BlogContextFactory())
      {
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var subscriptionTable1 = GetPushNotificationSubscriptionTableInstance(endPoint: endPoint);
          context.Add(subscriptionTable1);
          context.SaveChanges();
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          var subscriber = await unitOfWork.Subscribers.GetPushNotificationSubscriptionAsync(endPoint);

          // Assert
          Assert.NotNull(subscriber);
        }
      }
    }

    [Fact]
    public async Task Get_PushNotification_Count_Should_Be_2()
    {
      using (var factory = new BlogContextFactory())
      {
        using (var context = factory.CreateBlogContext())
        {
          // Arrange
          var unitOfWork = new UnitOfWork(context, mapper);
          var subscriptionTable1 = GetPushNotificationSubscriptionTableInstance();
          var subscriptionTable2 = GetPushNotificationSubscriptionTableInstance();
          context.Add(subscriptionTable1);
          context.Add(subscriptionTable2);
          context.SaveChanges();
        }

        using (var context = factory.CreateBlogContext())
        {
          // Act
          var unitOfWork = new UnitOfWork(context, mapper);
          var subscribersCount = await unitOfWork.Subscribers.GetPushNotificationSubscribersCountAsync();

          // Assert
          Assert.Equal(2, subscribersCount);
        }
      }
    }

    private PushNotificationSubscriptionDto GetPushNotificationSubscriptionDtoInstance(
    string authSecret = "auth secret",
      string endPoint = "end point",
      string key = "key"
      )
    {
      return new PushNotificationSubscriptionDto
      {
        AuthSecret = authSecret,
        EndPoint = endPoint,
        Key = key,
      };
    }

    private PushNotificationSubscriptionTable GetPushNotificationSubscriptionTableInstance(
      string authSecret = "auth secret",
      string endPoint = "end point",
      string key = "key"
      )
    {
      return new PushNotificationSubscriptionTable
      {
        AuthSecret = authSecret,
        EndPoint = endPoint,
        Key = key,
      };
    }
  }
}
