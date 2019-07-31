using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Extensions;
using MyBlog.Model;
using MyBlog.Repository.Data;
using MyBlog.ViewModel;

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
      PaggingResult<Blog> blogPage =
      unitOfWork.Blogs.GetBlogsPage(new BlogQuery()
      {
        Page = pageNumber
      });

      return View(blogPage);
    }

    [HttpGet("blog/{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
      var blog = await unitOfWork.Blogs.GetBlogAsync(slug);

      if (blog == null)
      {
        return NotFound();
      };

      var pageDescription = blog.Description.GetFirstParagraphTextFromHtml();
      if (string.IsNullOrEmpty(pageDescription))
      {
        pageDescription = "Bring your coffee and enjoy reading in-depth articles about programming.";
      }

      var blogDetailVM = new BlogDetailViewModel
      {
        Blog = blog,
        DisqusViewModel = new DisqusViewModel
        {
          PageTitle = blog.Title
        },
        PageDescription = pageDescription
      };

      return View(blogDetailVM);
    }   
  }
}