using Apitransac.Common;
using Apitransac.DTOs.Auth;
using Apitransac.Services.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Apitransac.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);

            var response = new ApiResponse<UserResponseDto>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status201Created,
                Data = result
            };

            return StatusCode
                (
                StatusCodes.Status201Created,
                response
                );
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            var response = new ApiResponse<LoginResponseDto>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Data = result
            };

            return Ok(response);
        }
    }
}

/***
 * {
  "email": "juan@example.com",
  "password": "MiPassword123!"
}
 */
