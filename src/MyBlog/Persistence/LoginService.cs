using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using MyBlog.Services;
using MyBlog.Settings;
using System;
using System.Text;

namespace MyBlog.Persistence
{
  public class LoginService : ILoginService
  {
    private readonly LoginSettings _loginSettings;

    public LoginService(IOptions<LoginSettings> loginSettings)
    {
      _loginSettings = loginSettings.Value;
    }
    public bool ValidateUser(string username, string password)
    {
      var saltBytes = Encoding.UTF8.GetBytes(_loginSettings.Salt);

      var hashBytes = KeyDerivation.Pbkdf2(
          password: password,
          salt: saltBytes,
          prf: KeyDerivationPrf.HMACSHA1,
          iterationCount: 1000,
          numBytesRequested: 256 / 8);

      var hashValue = BitConverter.ToString(hashBytes).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
      return hashValue == _loginSettings.Hash;
    }
  }
}
