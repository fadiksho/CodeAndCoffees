using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyBlog.DTO;
using MyBlog.Model;
using MyBlog.Repository.Data;
using MyBlog.Settings;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WebPush;

namespace MyBlog.Api
{
  [Authorize]
  [ApiController]
  [Route("api/subscribers")]
  public class SubscriberApiController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly PushNotificationSettings _pushNotificationSettings;
    public SubscriberApiController(IUnitOfWork unitOfWork,
      IOptions<PushNotificationSettings> config)
    {
      _unitOfWork = unitOfWork;
      _pushNotificationSettings = config.Value;
    }

    [AllowAnonymous]
    [HttpPost("PushNotificationSubscription")]
    public async Task<IActionResult> Create([FromBody] PushNotificationSubscriptionDto pushNotificationSubscription)
    {
      if (!ModelState.IsValid)
      {
        return new UnprocessableEntityObjectResult(ModelState);
      }
      var pushSubscription = new PushSubscription(pushNotificationSubscription.EndPoint, pushNotificationSubscription.Key, pushNotificationSubscription.AuthSecret);
      var vapidDetails = new VapidDetails(_pushNotificationSettings.Subject, _pushNotificationSettings.PublicKey, _pushNotificationSettings.PrivateKey);

      var webPushClient = new WebPushClient();
      webPushClient.SetVapidDetails(vapidDetails);

      await _unitOfWork.Subscribers.AddPushNotificationSubscriptionAsync(pushNotificationSubscription);
      await this._unitOfWork.SaveAsync();

      var payload = new PushNotificationPayload
      {
        Title = "Welcome",
        Body = "Now you won't miss any post :-)",
      };

      try
      {
        await webPushClient.SendNotificationAsync(pushSubscription,
        JsonConvert.SerializeObject(payload), vapidDetails);
      }
      catch (WebPushException exception)
      {
        var statusCode = exception.StatusCode;
        return StatusCode((int)statusCode, new { message = exception.Message });
      }

      return StatusCode(201);
    }

    [AllowAnonymous]
    [HttpPost("PushNotificationCancelSubscription")]
    public async Task<IActionResult> Remove([FromBody] PushNotificationSubscriptionBase dto)
    {
      if (dto == null)
      {
        return BadRequest();
      }

      if (!ModelState.IsValid)
      {
        return new UnprocessableEntityObjectResult(dto);
      }

      var subscription = await _unitOfWork.Subscribers
        .GetPushNotificationSubscriptionAsync(dto.EndPoint);

      if (subscription == null)
      {
        return NoContent();
      }

      await _unitOfWork.Subscribers
        .RemovePushNotificationSubscriptionAsync(dto.EndPoint);

      await _unitOfWork.SaveAsync();

      return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("CheckIfPushNotificationSubscriber")]
    public async Task<IActionResult> CheckIfSubscriber([FromBody] PushNotificationSubscriptionBase dto)
    {
      if (dto == null || !ModelState.IsValid)
      {
        return new JsonResult(new { isSubscribed = false });
      }

      var subscription = await _unitOfWork.Subscribers
        .GetPushNotificationSubscriptionAsync(dto.EndPoint);

      if (subscription == null)
      {
        return new JsonResult(new { isSubscribed = false });
      }

      return new JsonResult(new { isSubscribed = true });
    }

    [HttpPost("newPushNotification")]
    public async Task<IActionResult> PushNofitication([FromBody] PushNotificationPayload payload)
    {
      if (!ModelState.IsValid)
      {
        return new UnprocessableEntityObjectResult(payload);
      }

      // ToDo: Check if url is valid
      payload.Url = !string.IsNullOrEmpty(payload.Url) ? payload.Url :
        $"{Request.Scheme}://{Request.Host.Value}/";

      var messageSentCount = 0;
      var messageFaildCount = 0;

      var subscriptions = await _unitOfWork.Subscribers.GetPushNotificationSubscriptionAsync();
      var vapidDetails = new VapidDetails(_pushNotificationSettings.Subject, _pushNotificationSettings.PublicKey, _pushNotificationSettings.PrivateKey);
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
          await _unitOfWork.Subscribers.RemovePushNotificationSubscriptionAsync(subscriber.Endpoint);
        }
      }

      if (messageFaildCount > 0)
      {
        await _unitOfWork.SaveAsync();
      }

      return StatusCode(201, new
      {
        subscribersCount = messageSentCount,
        messageSentCount = messageSentCount - messageFaildCount,
        messageFaildCount
      });
    }

    [HttpGet("getSubscribersCount")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSubscribersCount()
    {
      var subscribersCount = await _unitOfWork.Subscribers.GetPushNotificationSubscribersCountAsync();

      return Ok(subscribersCount);
    }
  }
}
