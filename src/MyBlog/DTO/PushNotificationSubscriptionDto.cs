using System.ComponentModel.DataAnnotations;

namespace MyBlog.DTO
{
	public class PushNotificationSubscriptionDto : PushNotificationSubscriptionBase
  {
    [Required]
    public string Key { get; set; }
    [Required]
    public string AuthSecret { get; set; }
  }
}
