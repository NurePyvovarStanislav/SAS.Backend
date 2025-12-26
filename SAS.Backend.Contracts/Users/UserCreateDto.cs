using SAS.Backend.Contracts.Enums;

namespace SAS.Backend.Contracts.Users
{
    public record UserCreateDto
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public UserRole Role { get; init; } = UserRole.User;
        public string? Phone { get; init; }
        public Guid? FieldId { get; init; }
    }
}

