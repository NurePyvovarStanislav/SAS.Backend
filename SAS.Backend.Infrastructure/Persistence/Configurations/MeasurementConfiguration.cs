using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAS.Backend.Domain.Entities;

namespace SAS.Backend.Infrastructure.Persistence.Configurations
{
    public class MeasurementConfiguration : IEntityTypeConfiguration<Measurement>
    {
        public void Configure(EntityTypeBuilder<Measurement> builder)
        {
            builder.ToTable("Measurements");
            builder.HasKey(m => m.MeasurementId);

            builder.Property(m => m.Value)
                .IsRequired();

            builder.Property(m => m.MeasuredAt)
                .IsRequired();

            builder.HasOne(m => m.Sensor)
                .WithMany(s => s.Measurements)
                .HasForeignKey(m => m.SensorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

