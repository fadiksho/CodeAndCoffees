using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBlog.DTO
{
  public class BlogForCreatingDto
  {
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string Body { get; set; }
    [Required]
    public DateTime PublishedDate { get; set; }

    public List<string> Tags { get; set; } = new List<string>();
    public bool IsPublished { get; set; }
  }
}
