using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Model;
using MyBlog.Repository.Data;

namespace MyBlog.Controllers
{
  public class BlogController : Controller
  {
    private readonly IUnitOfWork unitOfWork;

    public BlogController(IUnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
    }
    [HttpGet("")]
    [HttpGet("blog/{pageNumber:int?}")]
    public IActionResult Index(int pageNumber = 1)
    {
      try
      {
        PaggingResult<Blog> blogPage =
        unitOfWork.Blogs.GetBlogsPage(new BlogQuery()
        {
          Page = pageNumber
        });
        return View(blogPage);
      }
      catch (Exception ex)
      {
        // ToDo: Logging
        throw ex;
      }
    }

    [HttpGet("{year}/{month}/{day}/{slug}")]
    public async Task<IActionResult> Detail(string year, string month, string day, string slug)
    {
      var fullSlug = $"{year}/{month}/{day}/{slug}";
      try
      {
        var blog = await unitOfWork.Blogs.GetBlogAsync(fullSlug);

        if (blog == null || !blog.IsPublished)
        {
          // Implement Blog Not Found
          return NotFound();
        };

        return View(blog);
      }
      catch
      {
        // ToDo: Logging
      }

      return Redirect("/");
    }
  }
}