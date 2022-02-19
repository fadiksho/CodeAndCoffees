using System.ComponentModel.DataAnnotations;

namespace MyBlog.ViewModel
{
  public class LoginViewModel
  {
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; } = false;
  }
}
