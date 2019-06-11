using MyBlog.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebPush;

namespace MyBlog.Repository
{
	public interface ISubscriberRepository
  {
    Task AddPushNotificationSubscriptionAsync(PushNotificationSubscriptionDto subscriptionDto);
    Task RemovePushNotificationSubscriptionAsync(string subscriptionEndPoint);
    Task<int> GetPushNotificationSubscribersCountAsync();

    Task<PushSubscription> GetPushNotificationSubscriptionAsync(string subscriptionEndPoint);
    Task<IEnumerable<PushSubscription>> GetPushNotificationSubscriptionAsync();
  }
}
