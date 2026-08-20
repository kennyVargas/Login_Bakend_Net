
using Apitransac.Models;
using Apitransac.Models.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;


namespace Apitransac.Services.RefreshTokens
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly JwtSettings _jwtSettings;
        public RefreshTokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public RefreshTokenResult CreateRefreshToken(int userId)
        {
            var token = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64));

            var tokenHash = HashToken(token);

            var entity = new RefreshToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(
                    _jwtSettings.RefreshTokenExpirationDays)
            };

            return new RefreshTokenResult
            {
                Token = token,
                Entity = entity
            };
        }

        public string GenerateToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(randomBytes);
        }

        public string HashToken(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);

            var hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }
    }
}
