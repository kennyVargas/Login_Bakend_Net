using Apitransac.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Apitransac.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Data = new
                {
                    Message = "Este endpoint es público."
                }
            });
        }

        [Authorize]
        [HttpGet("private")]
        public IActionResult Private()
        {
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Data = new
                {
                    Message = "Tienes acceso porque estás autenticado."
                }
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult Admin()
        {
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Data = new
                {
                    Message = "Tienes acceso porque eres administrador."
                }
            });
        }
    }
}
