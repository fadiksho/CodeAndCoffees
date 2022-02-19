using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Services;
using MyBlog.ViewModel;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyBlog.Controllers
{
  public class LoginController : Controller
  {
    private readonly ILoginService _loginService;

    public LoginController(ILoginService loginService)
    {
      _loginService = loginService;
    }

    [Route("/login")]
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
      ViewData[Constants.ReturnUrl] = returnUrl;
      return View();
    }

    [Route("/login")]
    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginAsync(string returnUrl, LoginViewModel model)
    {
      if (model is null || model.UserName is null || model.Password is null)
      {
        ModelState.AddModelError(string.Empty, "Username or password is invalid.");
        return View(nameof(Login), model);
      }

      if (!ModelState.IsValid || !_loginService.ValidateUser(model.UserName, model.Password))
      {
        ModelState.AddModelError(string.Empty, "Username or password is invalid.");
        return View(nameof(Login), model);
      }

      var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
      identity.AddClaim(new Claim(ClaimTypes.Name, model.UserName));

      var principle = new ClaimsPrincipal(identity);
      var properties = new AuthenticationProperties { IsPersistent = model.RememberMe };
      await HttpContext.SignInAsync(principle, properties).ConfigureAwait(false);

      return LocalRedirect(returnUrl ?? "/");
    }

    [Route("/logout")]
    public async Task<IActionResult> LogOutAsync()
    {
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
      return LocalRedirect("/");
    }
  }
}
