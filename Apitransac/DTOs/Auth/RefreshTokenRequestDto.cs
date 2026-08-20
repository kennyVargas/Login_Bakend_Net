using System.ComponentModel.DataAnnotations;

namespace Apitransac.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "El refresh token es obligatorio.")] 
        public string RefreshToken { get; set; } = string.Empty;
    }
}
