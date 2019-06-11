namespace MyBlog.Entity
{
	public class PushNotificationSubscriptionTable
  {
    public int Id { get; set; }

    public string EndPoint { get; set; }
    public string Key { get; set; }
    public string AuthSecret { get; set; }
  }
}
