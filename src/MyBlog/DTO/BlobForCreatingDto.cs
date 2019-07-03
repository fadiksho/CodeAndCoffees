using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MyBlog.DTO
{
  public class BlobForCreatingDto
  {
    [MaxLength(250)]
    public string Name { get; set; }
    [Required]
    public IFormFile File { get; set; }
  }
}
