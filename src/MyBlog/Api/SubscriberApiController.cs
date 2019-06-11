using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyBlog.DTO;
using MyBlog.Model;
using MyBlog.Repository.Data;
using MyBlog.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WebPush;

namespace MyBlog.Api
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/subscribers")]
  public class SubscriberApiController : Controller
  {
    private readonly IUnitOfWork unitOfWork;
    private readonly VapidSettings vapidSettings;
    public SubscriberApiController(IUnitOfWork unitOfWork, IOptions<AppSettings> config)
    {
      this.unitOfWork = unitOfWork;
      this.vapidSettings = config.Value.Vapid;
    }
    [AllowAnonymous]
    [HttpPost("PushNotificationSubscription")]
    public async Task<IActionResult> Create([FromBody] PushNotificationSubscriptionDto pushNotificationSubscription)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          return BadRequest();
        }
        var pushSubscription = new PushSubscription(pushNotificationSubscription.EndPoint, pushNotificationSubscription.Key, pushNotificationSubscription.AuthSecret);
        var vapidDetails = new VapidDetails(vapidSettings.Subject, vapidSettings.PublicKey, vapidSettings.PrivateKey);

        var webPushClient = new WebPushClient();
        webPushClient.SetVapidDetails(vapidDetails);

        await unitOfWork.Subscribers.AddPushNotificationSubscriptionAsync(pushNotificationSubscription);
        if (!await this.unitOfWork.SaveAsync())
        {
          return StatusCode(500);
        }

        var payload = new PushNotificationPayload
        {
          Title = "Welcome",
          Body = "Thank you for subscribing :-)",
        };
        try
        {
          await webPushClient.SendNotificationAsync(pushSubscription,
          JsonConvert.SerializeObject(payload), vapidDetails);
        }
        catch (WebPushException exception)
        {
          var statusCode = exception.StatusCode;
          return new StatusCodeResult((int)statusCode);
        }

      }
      catch
      {
        return StatusCode(500);
      }

      return StatusCode(201);
    }

    [AllowAnonymous]
    [HttpPost("PushNotificationCancelSubscription")]
    public async Task<IActionResult> Remove([FromBody] PushNotificationSubscriptionDto pushNotificationSubscription)
    {
      try
      {
        if (string.IsNullOrEmpty(pushNotificationSubscription.EndPoint))
        {
          return BadRequest();
        }
        var subscription = await unitOfWork.Subscribers
          .GetPushNotificationSubscriptionAsync(pushNotificationSubscription.EndPoint);
        if (subscription == null)
        {
          return NotFound();
        }
        await unitOfWork.Subscribers
          .RemovePushNotificationSubscriptionAsync(pushNotificationSubscription.EndPoint);

        if (!await unitOfWork.SaveAsync())
        {
          return StatusCode(500);
        }
      }
      catch
      {
        // ToDo: Logging
        return StatusCode(500);
      }

      return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("CheckIfPushNotificationSubscriber")]
    public async Task<IActionResult> CheckIfSubscriber([FromBody] PushNotificationSubscriptionDto pushNotificationSubscription)
    {
      try
      {
        if (string.IsNullOrEmpty(pushNotificationSubscription.EndPoint))
        {
          return BadRequest();
        }

        var subscription = await unitOfWork.Subscribers
          .GetPushNotificationSubscriptionAsync(pushNotificationSubscription.EndPoint);

        if (subscription == null)
        {
          return new JsonResult(new { isSubscribed = false });
        }

        return new JsonResult(new { isSubscribed = true });
      }
      catch
      {
        // ToDo: Logging
      }
      return StatusCode(500);
    }

    [HttpPost("newPushNotification")]
    public async Task<IActionResult> PushNofitication([FromBody]PushNotificationPayload payload)
    {
      var messageSentCount = 0;
      var messageFaildCount = 0;
      try
      {
        var subscriptions = await unitOfWork.Subscribers.GetPushNotificationSubscriptionAsync();
        var vapidDetails = new VapidDetails(vapidSettings.Subject, vapidSettings.PublicKey, vapidSettings.PrivateKey);
        var webPushClient = new WebPushClient();
        webPushClient.SetVapidDetails(vapidDetails);

        foreach (var subscriber in subscriptions)
        {
          try
          {
            messageSentCount += 1;
            webPushClient.SendNotification(subscriber,
            JsonConvert.SerializeObject(payload), vapidDetails);
          }
          catch (Exception ex) when (ex.InnerException is WebPushException)
          {
            messageFaildCount += 1;
            await unitOfWork.Subscribers.RemovePushNotificationSubscriptionAsync(subscriber.Endpoint);
          }
        }
        if (messageFaildCount > 0 && !await unitOfWork.SaveAsync())
        {
          return StatusCode(500, new
          {
            subscribersCount = messageSentCount,
            messageSentCount = messageSentCount - messageFaildCount,
            messageFaildCount
          });
        }
        return StatusCode(201, new
        {
          subscribersCount = messageSentCount,
          messageSentCount = messageSentCount - messageFaildCount,
          messageFaildCount
        });
      }
      catch
      {
        return StatusCode(500, new
        {
          subscribersCount = messageSentCount,
          messageSentCount = messageSentCount - messageFaildCount,
          messageFaildCount
        });
      }
    }

    [HttpGet("getSubscribersCount")]
    public async Task<IActionResult> GetSubscribersCount()
    {
      try
      {
        var subscribersCount = await unitOfWork.Subscribers.GetPushNotificationSubscribersCountAsync();
        return Ok(subscribersCount);
      }
      catch
      {
        // ToDo: Logging
      }
      return StatusCode(500);
    }
  }
}
