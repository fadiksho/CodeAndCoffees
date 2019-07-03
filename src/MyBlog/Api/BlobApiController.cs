using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyBlog.Abstraction;
using MyBlog.DTO;
using MyBlog.Entity;
using MyBlog.Model;
using MyBlog.Repository.Data;
using MyBlog.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyBlog.Api
{
  //[Authorize]
  [Route("api/blob")]
  public class BlobApiController : Controller
  {
    private readonly IHostingEnvironment host;
    private readonly IUnitOfWork unitOfWork;
    private readonly IFileHelper fileHelper;
    private readonly WebSiteHostingSettings webHostingSettings;
    public BlobApiController(
      IOptions<AppSettings> config,
      IHostingEnvironment host,
      IUnitOfWork unitOfWork,
      IFileHelper fileHelper)
    {
      this.host = host;
      this.webHostingSettings = config.Value.WebSiteHosting;
      this.unitOfWork = unitOfWork;
      this.fileHelper = fileHelper;
    }
    [HttpPost]
    public async Task<IActionResult> UploadAsync(BlobForCreatingDto blob)
    {
      try
      {
        if (blob.File == null) return BadRequest("Null file");
        if (blob.File.Length == 0) return BadRequest("Empty file");
        if (blob.File.Length > 10 * 1024 * 1024) return BadRequest("Max file size exceeded");
        //  Validate file extenstion
        var fileExtenstion = Path.GetExtension(blob.File.FileName);
        if (!fileHelper.IsExtenstionSupported(fileExtenstion))
          return BadRequest("Invalid file type.");

        var folderPath = new string[] { "UploadedFiles", "Images", DateTime.Now.Year.ToString() };
        var uploadsFolerPath = Path.Combine(
          host.ContentRootPath, Path.Combine(folderPath));
        if (!Directory.Exists(uploadsFolerPath))
          Directory.CreateDirectory(uploadsFolerPath);

        var fileName = Path.GetRandomFileName();
        if (string.IsNullOrEmpty(blob.Name))
          blob.Name = fileName;

        var fileNameWithExtenstion = $"{fileName}{fileExtenstion}";
        var filePath = Path.Combine(uploadsFolerPath, fileNameWithExtenstion);
        var fileUrl = string.Join("/",
          this.webHostingSettings.Url, string.Join("/", folderPath) , fileNameWithExtenstion);

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
          });

        if (!await unitOfWork.SaveAsync())
          return StatusCode(500);

        return StatusCode(201);
      }
      catch
      {
        // ToDo: Logging
        return StatusCode(500);
      }
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