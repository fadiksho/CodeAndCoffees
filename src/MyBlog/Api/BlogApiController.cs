using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyBlog.DTO;
using MyBlog.Model;
using MyBlog.Repository.Data;

namespace MyBlog.Api
{
  [Authorize]
  [Route("api/blog")]
  public class BlogApiController : Controller
  {
    private readonly IUnitOfWork unitOfWork;

    public BlogApiController(IUnitOfWork unitOfWork)
    {
      this.unitOfWork = unitOfWork;
    }

    [HttpGet]
    public IActionResult GetBlogsPage(BlogQuery blogQuery)
    {
      try
      {
        if (ModelState.IsValid)
        {
          var date = DateTime.UtcNow;
          PaggingResult<Blog> blogPage =
        unitOfWork.Blogs.GetBlogsPage(blogQuery);
          return Ok(blogPage);
        }
        return BadRequest();
      }
      catch
      {
        // ToDo: Logging
      }
      return StatusCode(500);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBlog(int id)
    {
      try
      {
        var blog = await unitOfWork.Blogs.GetBlogAsync(id);
        if (blog == null)
          return NotFound();
        return Ok(blog);
      }
      catch
      {
        // ToDo: Logging
      }
      return StatusCode(500);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlog([FromBody]BlogForCreatingDto newBlog)
    {
      try
      {
        if (ModelState.IsValid)
        {
          await unitOfWork.Blogs.AddBlog(newBlog);
          if (!await unitOfWork.SaveAsync())
          {
            return StatusCode(500);
          }
          return StatusCode(201);
        }
        return BadRequest();
      }
      catch
      {
        // ToDo: Logging
      }
      return StatusCode(500);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBlog(int id,
      [FromBody]BlogForUpdatingDto updatedBlog)
    {
      try
      {
        if (ModelState.IsValid)
        {
          if (await unitOfWork.Blogs.GetBlogAsync(id) == null)
            return NotFound();

          await unitOfWork.Blogs.UpdateBlogAsync(id, updatedBlog);

          if (!await unitOfWork.SaveAsync())
            return StatusCode(500);

          return NoContent();
        }
        return BadRequest();
      }
      catch
      {
        // ToDo: Logging
      }
      return StatusCode(500);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlog(int id)
    {
      try
      {
        if (await unitOfWork.Blogs.GetBlogAsync(id) == null)
          return NotFound();

        await unitOfWork.Blogs.DeleteBlogAsync(id);

        if (!await unitOfWork.SaveAsync())
          return StatusCode(500);

        return NoContent();
      }
      catch
      {
        // ToDo: Logging
      }
      return StatusCode(500);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PartiallyUpdateBlog(int id,
      [FromBody] JsonPatchDocument<BlogForUpdatingDto> patchDoc)
    {
      try
      {
        if (patchDoc == null)
          return BadRequest();

        var blog = await unitOfWork.Blogs.GetBlogAsync(id);

        if (blog == null)
        {
          return NotFound();
        }

        var updatedBlogDto = Mapper.Map<BlogForUpdatingDto>(blog);

        patchDoc.ApplyTo(updatedBlogDto);

        await unitOfWork.Blogs.UpdateBlogAsync(id, updatedBlogDto);

        if (!await unitOfWork.SaveAsync())
          return StatusCode(500);

        return NoContent();
      }
      catch
      {
        // ToDo: Logging
      }
      return StatusCode(500);
    }
  }
}
