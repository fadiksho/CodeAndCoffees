using System.Text.RegularExpressions;

namespace MyBlog.Extensions
{
  public static class StringExtensions
  {
    public static string GetFirstParagraphTextFromHtml(this string html)
    {
      Match m = Regex.Match(html, @"<(\s{0,2})p(\s{0,2})>\s{0,2}(.+?)\s{0,2}<(\s{0,2})\/(\s{0,2})p(\s{0,2})>");
      if (m.Success) return m.Groups[3].Value;
      else return "";
    }
  }
}
