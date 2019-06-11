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

    public string GetFirstParagraphFromHtml(string html)
    {
      Match m = Regex.Match(html, @"<(\s{0,2})p(\s{0,2})>\s{0,2}(.+?)\s{0,2}<(\s{0,2})\/(\s{0,2})p(\s{0,2})>");
      if (m.Success) return m.Groups[3].Value;
      else return "";
    }
  }
}
