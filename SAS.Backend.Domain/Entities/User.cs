using SAS.Backend.Domain.Enums;

namespace SAS.Backend.Domain.Entities
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public UserRole Role { get; set; }
        public string? Phone { get; set; }

        public ICollection<Field> Fields { get; set; } = new List<Field>();
    }
}
