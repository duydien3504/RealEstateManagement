using Microsoft.IdentityModel.Tokens;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RealEstateSystem.Infrastructure.Security
{
    public class JwtTokenProvider : ITokenProvider
    {
        private readonly IConfiguration _Configuration;

        public JwtTokenProvider(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public string GenerateAccessToken(User user)
        {
            string secretKey = _Configuration["Jwt:SecretKey"];
            string expiredAccessToken = _Configuration["Jwt:AccessToken:ExpirationMinutes"];

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("RoleId", user.RoleId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "RealEstateApp",
                audience: "RealEstateUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(expiredAccessToken)),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
