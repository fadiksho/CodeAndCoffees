namespace MyBlog.Settings
{
  public class PersistenceSettings
  {
    public bool UseSqlLite { get; set; }
    public bool UseSqlServer { get; set; }

    public PersistenceConnectionStrings ConnectionStrings { get; set; }
      = new PersistenceConnectionStrings();

    public class PersistenceConnectionStrings
    {
      public string SqlLite { get; set; } = string.Empty;
      public string SqlServer { get; set; } = string.Empty;
    }
  }
}
