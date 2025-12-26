namespace SAS.Backend.Contracts.Users
{
    public record UserDto(Guid UserId, string Email, string FullName, SAS.Backend.Contracts.Enums.UserRole Role, string? Phone, Guid? FieldId);
}

