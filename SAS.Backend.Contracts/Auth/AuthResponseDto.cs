using SAS.Backend.Contracts.Users;

namespace SAS.Backend.Contracts.Auth
{
    public record AuthResponseDto(string AccessToken, DateTime AccessTokenExpiresAt, string RefreshToken, DateTime RefreshTokenExpiresAt, UserDto User);
}

