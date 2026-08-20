using Apitransac.Models;

namespace Apitransac.Services.Jwt
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user, IEnumerable<string> roles);

        DateTime GetAccessTokenExpiration();
    }
}
