using MyBlog.Services;
using System.Text;

namespace MyBlog.Persistence
{
  public class URLHelper : IURLHelper
  {
    public string ToFriendlyUrl(string url)
    {
      url = url.Trim().ToLower();
      StringBuilder friendlyUrl = new StringBuilder();
      bool shouldAppend = true;
      for (int i = 0; i < url.Length; i++)
      {
        if (url[i] == ' ')
        {
          friendlyUrl.Append('-');
          shouldAppend = false;
        }
        else if (url[i] == '&')
        {
          if (shouldAppend)
            friendlyUrl.Append('-');
          friendlyUrl.Append("and");

          if (i != url.Length - 1) {
            friendlyUrl.Append('-');
            shouldAppend = false;
          }
          else shouldAppend = false;

        }
        else if ((url[i] >= '0' && url[i] <= '9') ||
                 (url[i] >= 'a' && url[i] <= 'z') ||
                 (url[i] >= 'A' && url[i] <= 'Z'))
        {
          friendlyUrl.Append(url[i]);
          shouldAppend = true;
        }
        else if (shouldAppend && i != url.Length - 1)
        {
          friendlyUrl.Append('-');
          shouldAppend = false;
        }
      }
      return friendlyUrl.ToString();
    }
  }
}
