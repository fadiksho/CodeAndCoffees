using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyBlog.DTO;
using MyBlog.Model;
using MyBlog.Repository.Data;
using MyBlog.Services;

namespace MyBlog.Api
{
  [Authorize]
  [Route("api/blog")]
  public class BlogApiController : Controller
  {
    private readonly IUnitOfWork unitOfWork;
    private readonly IURLHelper urlHelper;
    private readonly IMapper mapper;
    public BlogApiController(
      IUnitOfWork unitOfWork,
      IURLHelper urlHelper,
      IMapper mapper)
    {
      this.unitOfWork = unitOfWork;
      this.urlHelper = urlHelper;
      this.mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetBlogsPage(BlogQuery blogQuery)
    {
      PaggingResult<Blog> blogPage =
          unitOfWork.Blogs.GetBlogsPage(blogQuery);

      return Ok(blogPage);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBlog(int id)
    {
      var blogExist = await unitOfWork.Blogs
        .IsBlogExistByIdAsync(id);

      if (!blogExist)
      {
        return NotFound();
      }

      var blog = await unitOfWork.Blogs.GetBlogAsync(id);

      return Ok(blog);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlog([FromBody]BlogForCreatingDto newBlog)
    {
      if (newBlog == null)
      {
        return BadRequest();
      }

      if (ModelState.IsValid)
      {
        var slug = urlHelper.ToFriendlyUrl(newBlog.Title);
        var isUniqueSlug =
          await unitOfWork.Blogs.IsBlogSlugUniqueAsync(slug);

        if (isUniqueSlug)
        {
          await unitOfWork.Blogs.AddBlog(newBlog);
          await unitOfWork.SaveAsync();
          return StatusCode(201);
        }
        else
        {
          ModelState.AddModelError("Title", "This title has already has been taken.");
        }
      }
      return new UnprocessableEntityObjectResult(ModelState);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBlog(int id,
      [FromBody]BlogForUpdatingDto updatedBlog)
    {
      if (updatedBlog == null)
      {
        return BadRequest();
      }

      if (!ModelState.IsValid)
      {
        return new UnprocessableEntityObjectResult(ModelState);
      }

      var blogExist = await unitOfWork.Blogs
        .IsBlogExistByIdAsync(id);

      if (!blogExist)
      {
        return NotFound();
      }

      await unitOfWork.Blogs.UpdateBlogAsync(id, updatedBlog);
      await unitOfWork.SaveAsync();

      return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlog(int id)
    {
      var blogExist = await unitOfWork.Blogs
        .IsBlogExistByIdAsync(id);

      if (!blogExist)
      {
        return NotFound();
      }

      await unitOfWork.Blogs.DeleteBlogAsync(id);
      await unitOfWork.SaveAsync();

      return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PartiallyUpdateBlog(int id,
      [FromBody] JsonPatchDocument<BlogForUpdatingDto> patchDoc)
    {
      if (patchDoc == null)
      {
        return BadRequest();
      }

      var blogExist = await unitOfWork.Blogs
        .IsBlogExistByIdAsync(id);

      if (!blogExist)
      {
        return NotFound();
      }

      var blog = await unitOfWork.Blogs.GetBlogAsync(id);
      var updatedBlogDto = mapper.Map<BlogForUpdatingDto>(blog);

      TryValidateModel(updatedBlogDto);

      if (ModelState.IsValid)
      {
        var slug = urlHelper.ToFriendlyUrl(updatedBlogDto.Title);
        var isUniqueSlug =
          await unitOfWork.Blogs.IsBlogSlugUniqueAsync(slug);
        if (isUniqueSlug)
        {
          patchDoc.ApplyTo(updatedBlogDto);

          await unitOfWork.Blogs.UpdateBlogAsync(id, updatedBlogDto);
          await unitOfWork.SaveAsync();

          return NoContent();
        }
        else
        {
          ModelState.AddModelError("Title", "This title has already has been taken.");
        }
      }

      return new UnprocessableEntityObjectResult(ModelState);
    }
  }
}
