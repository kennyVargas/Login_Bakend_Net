using Apitransac.DTOs.Auth;

namespace Apitransac.Services.Auth
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(RegisterRequestDto request); 
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}
