using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Abstraction;
using MyBlog.DTO;
using MyBlog.Entity;
using MyBlog.Model;
using MyBlog.Repository.Data;
using MyBlog.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace MyBlog.Api
{
  [Authorize]
  [ApiController]
  [Route("api/blob")]
  public class BlobApiController : ControllerBase
  {
    private readonly IWebHostEnvironment host;
    private readonly IUnitOfWork unitOfWork;
    private readonly IFileHelper fileHelper;
    private readonly ILogger<BlogApiController> logger;
    public BlobApiController(
      IWebHostEnvironment host,
      IUnitOfWork unitOfWork,
      ILogger<BlogApiController> logger,
      IFileHelper fileHelper)
    {
      this.host = host;
      this.unitOfWork = unitOfWork;
      this.fileHelper = fileHelper;
      this.logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> UploadAsync(BlobForCreatingDto blob)
    {
      if (!ModelState.IsValid)
      {
        return new UnprocessableEntityObjectResult(ModelState);
      }

      if (blob.File.Length == 0) return BadRequest("Empty file");
      if (blob.File.Length > 10 * 1024 * 1024) return BadRequest("Max file size exceeded");

      
      //  Validate file extenstion
      var fileExtenstion = Path.GetExtension(blob.File.FileName);
      if (!fileHelper.IsExtenstionSupported(fileExtenstion))
      {
        return BadRequest("Invalid file type.");
      }
      var folderPath = new string[]
      {
        "UploadedFiles",
        "Images",
        DateTime.Now.Year.ToString()
      };
      var uploadsFolerPath = Path.Combine(
        host.ContentRootPath, Path.Combine(folderPath));
      try
      {
        if (!Directory.Exists(uploadsFolerPath))
        {
          Directory.CreateDirectory(uploadsFolerPath);
        }

        var fileName = Path.GetRandomFileName();
        if (string.IsNullOrEmpty(blob.Name))
        {
          blob.Name = fileName;
        }

        var fileNameWithExtenstion = $"{fileName}{fileExtenstion}";
        var filePath = Path.Combine(uploadsFolerPath, fileNameWithExtenstion);

        var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        var fileUrl = string.Join("/",
          baseUrl, string.Join("/", folderPath), fileNameWithExtenstion);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          await blob.File.CopyToAsync(stream);
        }

        await unitOfWork.Blobs.AddBlobAsync(
          new BlobTable
          {
            Name = blob.Name,
            CreatedDate = DateTime.Now,
            FileName = fileNameWithExtenstion,
            FilePath = filePath,
            URL = fileUrl,
            FileSize = blob.File.Length
          }
        );
        await this.unitOfWork.SaveAsync();

        return StatusCode(201);
      }
      catch (IOException ex)
      {
        logger.LogWarning(ex.Message, ex);
        return StatusCode(409, new { message = "The server is busy now try again later." });
      }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUploadedFile(int id)
    {
      if (!await unitOfWork.Blobs.BlobExistAsync(id))
        return NotFound();

      var blob = await unitOfWork.Blobs.GetBlobAsync(id);
      return Ok(blob);
    }

    [HttpGet]
    public IActionResult GetUploadedFiles(PaggingQuery query)
    {
      PaggingResult<Blob> blogPage =
        unitOfWork.Blobs.GetBlobPage(query);

      return Ok(blogPage);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFileAsync(int id)
    {
      if (!await unitOfWork.Blobs.BlobExistAsync(id))
      {
        return NotFound();
      }

      var blobPath = await unitOfWork.Blobs.GetBlobPathAsync(id);

      FileInfo myfileinf = new FileInfo(blobPath);
      if (myfileinf.Exists)
      {
        try
        {
          myfileinf.Delete();
        }
        catch (IOException ex)
        {
          logger.LogWarning(ex.Message, ex);
          return StatusCode(409, new { message = "The server is busy now try again later." });
        }
      }

      await unitOfWork.Blobs.DeleteBlobAsync(id);
      await unitOfWork.SaveAsync();

      return NoContent();
    }
  }
}