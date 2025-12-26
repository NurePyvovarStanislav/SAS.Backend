using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAS.Backend.Domain.Entities;

namespace SAS.Backend.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.UserId);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(u => u.Phone)
                .HasMaxLength(20);

            builder.Property(u => u.FieldId)
                .IsRequired(false);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.HasOne(u => u.Field)
                .WithMany(f => f.Users)
                .HasForeignKey(u => u.FieldId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

