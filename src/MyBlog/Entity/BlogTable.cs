using System;

namespace MyBlog.Entity
{
  public class BlogTable
  {
    public int Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public string Body { get; set; }
    public string Tags { get; set; }
    public string KeyWords { get; set; }
    public DateTime PublishedDate { get; set; }
    public bool IsPublished { get; set; }

    public string Slug { get; set; }
  }
}
