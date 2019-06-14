namespace IDP.Services
{
  public class AppSettings
  {
    public ConnectionStrings ConnectionStrings { get; set; }
    public AdminUser AdminUser { get; set; }
  }
  public class ConnectionStrings
  {
    public string DefaultConnection { get; set; }
  }
  public class AdminUser
  {
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ClaimName { get; set; }
    public string GivenName { get; set; }
    public string Email { get; set; }
  }
}
