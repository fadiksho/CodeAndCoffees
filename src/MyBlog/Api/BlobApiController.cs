using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyBlog.Abstraction;
using MyBlog.Entity;
using MyBlog.Model;
using MyBlog.Repository.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyBlog.Api
{
  [Authorize]
  [Route("api/blob")]
  public class BlobApiController : Controller
  {
    private readonly IHostingEnvironment host;
    private readonly IConfiguration configuration;
    private readonly IUnitOfWork unitOfWork;

    public BlobApiController(
      IConfiguration configuration,
      IHostingEnvironment host,
      IUnitOfWork unitOfWork)
    {
      this.host = host;
      this.configuration = configuration;
      this.unitOfWork = unitOfWork;
    }
    [HttpPost]
    public async Task<IActionResult> UploadAsync(IFormFile file)
    {
      try
      {
        if (file == null) return BadRequest("Null file");
        if (file.Length == 0) return BadRequest("Empty file");
        if (file.Length > 10 * 1024 * 1024) return BadRequest("Max file size exceeded");
        var uploadsFolerPath = Path.Combine(host.WebRootPath, "uploads", "images");
        if (!Directory.Exists(uploadsFolerPath))
        {
          Directory.CreateDirectory(uploadsFolerPath);
        }
        var fileName = $"{DateTime.Now.ToShortDateString().Replace('/', '-')}_{Path.GetFileNameWithoutExtension(file.FileName.Replace("/", string.Empty))}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolerPath, fileName);
        var fileUrl = string.Join("/", configuration["WebSiteHosting:Url"], "uploads", "images", fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          await file.CopyToAsync(stream);
        }
        await unitOfWork.Blobs.AddBlobAsync(
          new BlobTable
          {
            FileName = fileName,
            FilePath = filePath,
            URL = fileUrl,
            FileSize = file.Length
          });
        if (!await unitOfWork.SaveAsync())
        {
          return StatusCode(500);
        }
        return StatusCode(201);
      }
      catch
      {
        // ToDo: Logging
      }
      return StatusCode(500);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUploadedFile(int id)
    {
      try
      {
        if (!await unitOfWork.Blobs.BlobExistAsync(id))
          return NotFound();

        var blob = await unitOfWork.Blobs.GetBlobAsync(id);
        return Ok(blob);
      }
      catch
      {
        // ToDo: Logging
      }
      return StatusCode(500);
    }

    [HttpGet]
    public IActionResult GetUploadedFiles(PaggingQuery query)
    {
      try
      {
        if (ModelState.IsValid)
        {
          PaggingResult<Blob> blogPage =
        unitOfWork.Blobs.GetBlobPageAsync(query);
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFileAsync(int id)
    {
      try
      {
        if (!await unitOfWork.Blobs.BlobExistAsync(id))
          return NotFound();

        // Delete The blob from the disk
        var blob = await unitOfWork.Blobs.GetBlobAsync(id);
        FileInfo myfileinf = new FileInfo(blob.FilePath);

        if (myfileinf.Exists)
          myfileinf.Delete();

        // Remove The blob from the db
        await unitOfWork.Blobs.DeleteBlob(id);
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