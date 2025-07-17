using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tasker.API.Controllers.Base;
using Tasker.API.Services;
using Tasker.API.Models.Transfer.User;

namespace Tasker.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, IMapper mapper, ITokenService tokenService, IConfiguration configuration) : base(mapper, userService, configuration)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseModel>> Login([FromBody] LoginRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var user = await _userService.AuthenticateAsync(model);
            if (user == null)
                return BadRequest("Username or Password is invalid");

            var response = _mapper.Map<LoginResponseModel>(user);

            response.Roles = await _userService.GetRolesForUserAsync(user) ?? new List<string>();

            var token = await _tokenService.GenerateAccessToken(user);

            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["TokenSettings:TokenExpirationMinutes"]))
            });

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest model)
        {
            var result = await _userService.RegisterAsync(model);

            if (!result.Succeeded)
                return BadRequest();

            return Ok();
        }
    }
}
