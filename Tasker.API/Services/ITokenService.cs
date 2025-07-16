using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Tasker.API.Models.Database;

namespace Tasker.API.Services
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(TaskerUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string _jwtSecret;
        private readonly UserManager<TaskerUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TokenService(IConfiguration configuration, UserManager<TaskerUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _configuration = configuration;
            _jwtSecret = _configuration["TokenSettings:Secret"] ?? throw new ArgumentNullException("A JWT secret must be set!");
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string> GenerateAccessToken(TaskerUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            // Add each role as a separate Role claim
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("TokenSettings:TokenExpirationMinutes")),
                SigningCredentials = credentials,
                Issuer = _configuration["TokenSettings:Issuer"],
                Audience = _configuration["TokenSettings:Audience"]
            };

            var handler = new JsonWebTokenHandler();
            return handler.CreateToken(tokenDescriptor);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }

}
