using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Application.Common.Settings;
using SAS.Backend.Domain.Entities;

namespace SAS.Backend.Infrastructure.Integration.Authentication
{
    public class JwtService : ITokenService
    {
        private readonly JwtOptions _options;

        public JwtService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public TokenPair GenerateTokenPair(User user)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);
            var jwtToken = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: accessTokenExpiresAt,
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenDays);

            return new TokenPair(accessToken, refreshToken, accessTokenExpiresAt, refreshTokenExpiresAt);
        }
    }
}
