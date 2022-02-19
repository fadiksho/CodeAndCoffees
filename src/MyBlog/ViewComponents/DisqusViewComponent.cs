using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyBlog.Services;
using MyBlog.Settings;
using MyBlog.ViewModel;
using System;

namespace MyBlog.ViewComponents
{
  public class DisqusViewComponent : ViewComponent
  {
    private readonly CommentSettings _commentSettings;

    public DisqusViewComponent(IOptions<CommentSettings> config)
    {
      _commentSettings = config.Value;
    }

    public IViewComponentResult Invoke(DisqusViewModel disqusOptions)
    {
      if (string.IsNullOrEmpty(disqusOptions.PageTitle))
      {
        throw new ArgumentException("Comment Title Was Null or Empty!");
      }

      if (!string.IsNullOrEmpty(_commentSettings.DisqusShortName))
      {
        disqusOptions.ShortName = _commentSettings.DisqusShortName;
      }

      if (string.IsNullOrEmpty(disqusOptions.PageUrl))
      {
        disqusOptions.PageUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";
      }

      if (string.IsNullOrEmpty(disqusOptions.PageIdentifier))
      {
        disqusOptions.PageIdentifier = $"{Request.Scheme}://{Request.Host}{Request.Path}";
      }

      return View("Disqus", disqusOptions);
    }
  }
}