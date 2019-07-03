using System;

namespace MyBlog.Model
{
  public class Blob
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; }
    public string URL { get; set; }
    public long FileSize { get; set; }
  }
}
