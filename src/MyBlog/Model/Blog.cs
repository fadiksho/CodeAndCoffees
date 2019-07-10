using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MyBlog.Model
{
  public class Blog
  {
    public int Id { get; set; }

    public string Title { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public string Body { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public DateTime PublishedDate { get; set; }
    public bool IsPublished { get; set; }
  }
}
