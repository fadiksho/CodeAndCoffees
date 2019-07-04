using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyBlog.DTO;
using MyBlog.Entity;
using MyBlog.Persistence.Data;
using MyBlog.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebPush;

namespace MyBlog.Persistence
{
	public class SubscriberRepository : ISubscriberRepository
  {
    private readonly BlogContext context;
    private readonly IMapper mapper;
    public SubscriberRepository(
      BlogContext context,
      IMapper mapper)
    {
      this.context = context;
      this.mapper = mapper;
    }

    public async Task AddPushNotificationSubscriptionAsync(PushNotificationSubscriptionDto subscriptionDto)
    {
      var pushNotificationEntity = mapper.Map<PushNotificationSubscriptionTable>(subscriptionDto);

      await context.PushNotificationSubscriptions.AddAsync(pushNotificationEntity);
    }

    public async Task<IEnumerable<PushSubscription>> GetPushNotificationSubscriptionAsync()
    {
      var subscriptionEntity = await context.PushNotificationSubscriptions.ToListAsync();
      return mapper.Map<IEnumerable<PushSubscription>>(subscriptionEntity);
    }

    public async Task RemovePushNotificationSubscriptionAsync(string subscriptionEndPoint)
    {
      var subscription = await context.PushNotificationSubscriptions
        .Where(c => c.EndPoint == subscriptionEndPoint)
        .FirstOrDefaultAsync();

      context.PushNotificationSubscriptions.Remove(subscription);
    }

    public async Task<PushSubscription> GetPushNotificationSubscriptionAsync(string subscriptionEndPoint)
    {
      var subscriptionEntity = await context.PushNotificationSubscriptions
        .Where(c => c.EndPoint == subscriptionEndPoint)
        .FirstOrDefaultAsync();
      
      return mapper.Map<PushSubscription>(subscriptionEntity);
    }

    public async Task<int> GetPushNotificationSubscribersCountAsync()
    {
      var subscribersCount = await context.PushNotificationSubscriptions.CountAsync();
      return subscribersCount;
    }
  }
}
