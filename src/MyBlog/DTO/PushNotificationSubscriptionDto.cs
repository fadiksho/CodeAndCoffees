using System.ComponentModel.DataAnnotations;

namespace MyBlog.DTO
{
	public class PushNotificationSubscriptionDto
  {
    [Required]
    public string EndPoint { get; set; }
    [Required]
    public string Key { get; set; }
    [Required]
    public string AuthSecret { get; set; }
  }
}
