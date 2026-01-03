using SAS.Backend.Domain.Entities;

namespace SAS.Backend.Application.Common.Interfaces
{
    public record TokenPair(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt, DateTime RefreshTokenExpiresAt);

    public interface ITokenService
    {
        TokenPair GenerateTokenPair(User user);
    }
}

