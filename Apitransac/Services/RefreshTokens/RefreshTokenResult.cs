using Apitransac.Models;

namespace Apitransac.Services.RefreshTokens
{
    public class RefreshTokenResult
    {
        public string Token { get; set; } = string.Empty;

        public RefreshToken Entity { get; set; } = null!;
    }
}
