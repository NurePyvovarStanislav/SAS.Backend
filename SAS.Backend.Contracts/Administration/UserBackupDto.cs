using SAS.Backend.Contracts.Enums;

namespace SAS.Backend.Contracts.Administration
{
    public sealed record UserBackupDto
    {
        public Guid UserId { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public UserRole Role { get; init; }
        public string? Phone { get; init; }
        public Guid? FieldId { get; init; }
        public bool IsActive { get; init; }
    }
}
