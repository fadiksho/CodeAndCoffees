using System.ComponentModel.DataAnnotations;

namespace MyBlog.Model
{
  public class PushNotificationPayload
  {
    [Required]
    public string Title { get; set; }
    [Required]
    public string Body { get; set; }
    public string Url { get; set; }
  }
}
