using Application.Requests;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Exchanges email and password for an access token (JWT).</summary>
        /// <response code="200">Token, expiration, role and seller id (for Seller users).</response>
        /// <response code="401">Invalid email or password (the response does not say which).</response>
        /// <response code="429">Too many attempts from this client; try again later.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting(RateLimitPolicies.Login)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var response = await _authService.LoginAsync(request, cancellationToken);

            if (response == null)
            {
                return Problem(title: "Invalid email or password.", statusCode: StatusCodes.Status401Unauthorized);
            }

            return Ok(response);
        }
    }
}
