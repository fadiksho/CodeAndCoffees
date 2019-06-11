using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyBlog.Entity;
using MyBlog.Services;
using MyBlog.ViewModel;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Controllers
{
  public class UserController : Controller
  {
    private readonly UserManager<User> userManger;
    private readonly IPasswordHasher<User> passwordHasher;
    private readonly TokenSettings tokenSetting;

    public UserController(
      UserManager<User> userManager,
      IPasswordHasher<User> passwordHasher,
      IOptions<AppSettings> config
      )
    {
      this.userManger = userManager;
      this.passwordHasher = passwordHasher;
      this.tokenSetting = config.Value.Token;
    }
    [HttpPost("api/auth/token")]
    [EnableCors("EnableCors")]
    public async Task<IActionResult> GenerateToken([FromBody]LoginViewModel model)
    {
      if (ModelState.IsValid != true) return BadRequest(ModelState.Values);

      var user = await this.userManger.FindByNameAsync(model.UserName);
      if (user == null) return BadRequest("Wrong Credential!");

      if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password)
      != PasswordVerificationResult.Success) return BadRequest("Wrong Credential!");

      var claims = new[]
      {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
      };
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSetting.Key));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
        issuer: tokenSetting.Issuer,
        audience: tokenSetting.Audience[0],
        claims: claims,
        notBefore: DateTime.UtcNow,
        expires: DateTime.UtcNow.AddDays(1),
        signingCredentials: creds
      );

      return Ok(new
      {
        token = new JwtSecurityTokenHandler().WriteToken(token),
      });
    }
  }
}
