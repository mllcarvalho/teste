using Microsoft.AspNetCore.Mvc;
using Oficina.Common.Application.IServices;

namespace Oficina.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthAppService _appService;

        public AuthController(IAuthAppService appService)
        {
            _appService = appService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token = _appService.Authenticate(request.Username, request.Password);

            if (token == null)
                return Unauthorized();

            return Ok(new { token });
        }
    }

    public record LoginRequest(string Username, string Password);
}
