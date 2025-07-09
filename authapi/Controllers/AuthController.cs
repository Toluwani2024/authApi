
 using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using authapi.DTOs;
using authapi.IServices;

namespace authapi.Controllers
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
            public async Task<IActionResult> Register(UserSignupDto request)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var response = await _authService.RegisterUserAsync(request);
                if (!response.success)
                {
                    return BadRequest(response.message);
                }
                return Ok(response.message);
            }
            [HttpPost("login")]
            public async Task<IActionResult> Login(UserLoginDto request)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var response = await _authService.LoginUserAsync(request);
                if (!response.success)
                {
                    return BadRequest(response.message);
                }
                return Ok(response.token);
            }


        }
}


