using AutoMapper;
using MyBlog.DTO;
using MyBlog.Entity;
using WebPush;

namespace MyBlog.MappingProfiles
{
  public class PushNotificationProfile : Profile
  {
    public PushNotificationProfile()
    {
      // Dto to Entity
      CreateMap<PushNotificationSubscriptionDto, PushNotificationSubscriptionTable>();

      // Entity to Model
      CreateMap<PushNotificationSubscriptionTable, PushSubscription>()
        .ForMember(a => a.P256DH, b => b.MapFrom(c => c.Key))
        .ForMember(a => a.Auth, b => b.MapFrom(c => c.AuthSecret));
    }
  }
}
