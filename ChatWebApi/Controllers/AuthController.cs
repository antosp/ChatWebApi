using Microsoft.AspNetCore.Mvc;
using ChatWebApi.Models;
using ChatWebApi.Services;

namespace ChatWebApi.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("api/auth/login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var response = _userService.Authenticate(request.Username, request.Password);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [HttpPost]
        [Route("api/auth/register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var response = _userService.Create(request.Username, request.Password, request.FirstName, request.LastName);

            return Ok(response);
        }
    }
}
