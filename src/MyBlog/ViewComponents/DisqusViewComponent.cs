using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyBlog.Services;
using MyBlog.ViewModel;

namespace MyBlog.ViewComponents
{
  public class DisqusViewComponent : ViewComponent
  {
    private readonly DisqusSettings disqusSettings;
    
    public DisqusViewComponent(IOptions<AppSettings> config)
    {
      disqusSettings = config.Value.Disqus;
    }
    
    public IViewComponentResult Invoke(DisqusViewModel disqusOptions)
    {
      if (!string.IsNullOrEmpty(disqusSettings.ShortName))
      {
        disqusOptions.ShortName = disqusSettings.ShortName;
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