using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Services;
using MyBlog.ViewModel;
using System.Security.Claims;

namespace MyBlog.Api
{
  [Authorize]
  [ApiController]
  public class LoginController : ControllerBase
  {
    private readonly ILoginService _loginService;

    public LoginController(ILoginService loginService)
    {
      _loginService = loginService;
    }

    [Route("api/login")]
    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> LoginAsync(LoginViewModel model)
    {
      if (model is null || model.UserName is null || model.Password is null)
      {
        ModelState.AddModelError(string.Empty, "Username or password is invalid.");
        return new UnprocessableEntityObjectResult(ModelState);
      }

      if (!ModelState.IsValid)
      {
        return new UnprocessableEntityObjectResult(ModelState);
      }

      if (!_loginService.ValidateUser(model.UserName, model.Password))
      {
        ModelState.AddModelError("Error", "Username or password is invalid.");
        return new UnprocessableEntityObjectResult(ModelState);
      }

      var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
      identity.AddClaim(new Claim(ClaimTypes.Name, model.UserName));

      var principle = new ClaimsPrincipal(identity);
      var properties = new AuthenticationProperties { IsPersistent = model.RememberMe };
      await HttpContext.SignInAsync(principle, properties).ConfigureAwait(false);

      return Ok();
    }

    [Route("api/logout")]
    public async Task<IActionResult> LogOutAsync()
    {
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
      return Ok();
    }
  }
}
