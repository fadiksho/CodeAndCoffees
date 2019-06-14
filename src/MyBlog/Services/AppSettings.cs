namespace MyBlog.Services
{
	public class AppSettings
  {
    public ConnectionStrings ConnectionStrings { get; set; }
    public DisqusSettings Disqus { get; set; }
    public JwtBearerSettings JwtBearer { get; set; }
    public WebSiteHostingSettings WebSiteHosting { get; set; }
    public VapidSettings Vapid { get; set; }
  }
  public class ConnectionStrings
  {
    public string DefaultConnection { get; set; }
  }
  public class DisqusSettings
  {
    public string ShortName { get; set; }
  }
  public class JwtBearerSettings
  {
    public string Authority { get; set; }
    public string Audience { get; set; }
  }
  public class WebSiteHostingSettings
  {
    public string Url { get; set; }
    public bool RequirdHttps { get; set; }
  }
  public class VapidSettings
  {
    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }
    public string Subject { get; set; }
  }
}
