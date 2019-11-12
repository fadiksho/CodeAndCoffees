using Microsoft.AspNetCore.Mvc;

namespace MyBlog.Controllers
{
  public class StatusCodeController : Controller
  {
    [Route("/StatusCode/{code}")]
    public IActionResult Index(int code)
    {
      switch (code)
      {
        case 404:
          return View("NotFound");
        case 500:
          return View("Error");
        default:
          return View("Error");
      }
    }
  }
}
