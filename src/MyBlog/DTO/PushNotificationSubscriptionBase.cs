using System.ComponentModel.DataAnnotations;

namespace MyBlog.DTO
{
  public class PushNotificationSubscriptionBase
  {

    [Required]
    public string EndPoint { get; set; }
  }
}
