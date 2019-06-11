using System;
using System.Text;

namespace MyBlog.Extensions
{
  public class URLHelper
  {
    public static string ToFriendlyUrl(string urlToEncode)
    {
      urlToEncode = (urlToEncode ?? "").Trim().ToLower();

      StringBuilder url = new StringBuilder();

      foreach (char ch in urlToEncode)
      {
        switch (ch)
        {
          case ' ':
            url.Append('-');
            break;
          case '&':
            url.Append("and");
            break;
          case '\'':
          case ',':
          case '.':
            break;
          default:
            if ((ch >= '0' && ch <= '9') ||
                (ch >= 'a' && ch <= 'z'))
            {
              url.Append(ch);
            }
            else
            {
              url.Append('-');
            }
            break;
        }
      }

      return url.ToString();
    }

    public static string BuildBlogUrl(string urlToEncode, DateTime date)
    {
      urlToEncode = date.ToString("yyyy/MM/dd") + "/" + ToFriendlyUrl(urlToEncode);
      return urlToEncode;
    }
  }
}
