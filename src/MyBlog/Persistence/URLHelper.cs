using MyBlog.Services;
using System.Text;

namespace MyBlog.Persistence
{
  public class URLHelper : IURLHelper
  {
    // [InlineData(@"new: Blog Post", "new-blog-post")]
    public string ToFriendlyUrl(string url)
    {
      url = url.Trim().ToLower();
      StringBuilder friendlyUrl = new StringBuilder();
      bool shouldAppend = true;
      for (int i = 0; i < url.Length; i++)
      {
        if (shouldAppend && url[i] == ' ')
        {
          friendlyUrl.Append('-');
          shouldAppend = false;
        }
        else if (url[i] == '&')
        {
          if (shouldAppend)
            friendlyUrl.Append('-');
          friendlyUrl.Append("and");

          if (i != url.Length - 1)
          {
            friendlyUrl.Append('-');
            shouldAppend = false;
          }
        }
        else if (charAcceptedInFriendlyUrl(url[i]))
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

    private bool charAcceptedInFriendlyUrl(char charToCheck)
    {
      return (charToCheck >= '0' && charToCheck <= '9') ||
        (charToCheck >= 'a' && charToCheck <= 'z') ||
        (charToCheck >= 'A' && charToCheck <= 'Z');
    }
  }
}
