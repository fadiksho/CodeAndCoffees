using MyBlog.Services;
using System;
using System.Linq;

namespace MyBlog.Persistence
{
  public class FileHelper : IFileHelper
  {
    readonly string[] supportedExtenstion =
      { ".png", ".jpg", ".jpeg", ".gif", ".webp" };
    public bool IsExtenstionSupported(string extension)
    {
      return supportedExtenstion.Any(ex =>
        ex.Contains(extension, StringComparison.OrdinalIgnoreCase));
    }
  }
}
