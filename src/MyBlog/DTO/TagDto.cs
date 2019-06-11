using System.ComponentModel.DataAnnotations;

namespace MyBlog.DTO
{
  public class TagDto
  {
    [Required]
    public string Name { get; set; }
  }
}
