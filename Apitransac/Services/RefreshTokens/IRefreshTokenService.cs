using Apitransac.Models;

namespace Apitransac.Services.RefreshTokens
{
    public interface IRefreshTokenService
    {
        RefreshTokenResult CreateRefreshToken(int userId);
        string GenerateToken();

        string HashToken(string token);
    }
}
