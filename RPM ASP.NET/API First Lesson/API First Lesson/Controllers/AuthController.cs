using API_First_Lesson.DTO.Auth;
using API_First_Lesson.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;


namespace API_First_Lesson.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersionNeutral]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, ITokenService tokenService, IConfiguration config)
        {
            _userService = userService;
            _tokenService = tokenService;
            _configuration = config;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            try
            {
                var user = await _userService.RegisterAsync(dto.Username, dto.Password);
                var token = _tokenService.CreateAccessToken(user);

                return Ok(new LoginResponceDTO
                {
                    AccessToken = token,
                    ExpiresInMinutes = int.Parse(_configuration["Jwt:ExpiresMinutes"]!)
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [EnableRateLimiting("LoginPolicy")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _userService.ValidateCredentialsAsync(dto.Username, dto.Password);

            if (user is null)
                return Unauthorized(new { error = "Неверный логин или пароль" });

            var token = _tokenService.CreateAccessToken(user);

            return Ok(new LoginResponceDTO
            {
                AccessToken = token,
                ExpiresInMinutes = int.Parse(_configuration["Jwt:ExpiresMinutes"]!)
            });
        }
    }
}
