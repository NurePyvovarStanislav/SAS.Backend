using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAS.Backend.Domain.Entities;

namespace SAS.Backend.Infrastructure.Persistence.Configurations
{
    public class AlertConfiguration : IEntityTypeConfiguration<Alert>
    {
        public void Configure(EntityTypeBuilder<Alert> builder)
        {
            builder.ToTable("Alerts");
            builder.HasKey(a => a.AlertId);

            builder.Property(a => a.Level)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.Message)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.IsResolved)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(a => a.ResolvedAt)
                .IsRequired(false);

            builder.HasOne(a => a.Measurement)
                .WithOne(m => m.Alert)
                .HasForeignKey<Alert>(a => a.MeasurementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => a.MeasurementId)
                .IsUnique();
        }
    }
}

