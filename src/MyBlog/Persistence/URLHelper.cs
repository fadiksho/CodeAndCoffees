using System;
using System.Text;

namespace MyBlog.Persistence
{
  public class URLHelper
  {
    public string ToFriendlyUrl(string urlToEncode)
    {
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
    public string BuildBlogUrl(string urlToEncode, DateTime date)
    {
      urlToEncode = date.ToString("yyyy/MM/dd") + "/" + ToFriendlyUrl(urlToEncode);
      return urlToEncode;
    }
  }
}
