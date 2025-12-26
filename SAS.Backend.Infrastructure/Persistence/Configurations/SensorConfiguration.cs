using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAS.Backend.Domain.Entities;

namespace SAS.Backend.Infrastructure.Persistence.Configurations
{
    public class SensorConfiguration : IEntityTypeConfiguration<Sensor>
    {
        public void Configure(EntityTypeBuilder<Sensor> builder)
        {
            builder.ToTable("Sensors");
            builder.HasKey(s => s.SensorId);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.SensorType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(100);

            builder.Property(s => s.MinValue)
                .IsRequired();

            builder.Property(s => s.MaxValue)
                .IsRequired();

            builder.Property(s => s.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Active");

            builder.Property(s => s.InstalledAt)
                .IsRequired();

            builder.HasOne(s => s.Field)
                .WithMany(f => f.Sensors)
                .HasForeignKey(s => s.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Measurements)
                .WithOne(m => m.Sensor)
                .HasForeignKey(m => m.SensorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

