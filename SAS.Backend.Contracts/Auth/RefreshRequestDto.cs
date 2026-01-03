namespace SAS.Backend.Contracts.Auth
{
    public record RefreshRequestDto
    {
        public string RefreshToken { get; init; } = string.Empty;
    }
}

