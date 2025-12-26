using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAS.Backend.Domain.Entities;

namespace SAS.Backend.Infrastructure.Persistence.Configurations
{
    public class FieldConfiguration : IEntityTypeConfiguration<Field>
    {
        public void Configure(EntityTypeBuilder<Field> builder)
        {
            builder.ToTable("Fields");
            builder.HasKey(f => f.FieldId);

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(f => f.CropType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.Area)
                .IsRequired();

            builder.Property(f => f.Location)
                .HasMaxLength(500);

            builder.HasOne(f => f.User)
                .WithMany(u => u.Fields)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(f => f.Sensors)
                .WithOne(s => s.Field)
                .HasForeignKey(s => s.FieldId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

