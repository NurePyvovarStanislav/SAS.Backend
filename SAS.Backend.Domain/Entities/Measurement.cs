namespace SAS.Backend.Domain.Entities
{
    public class Measurement
    {
        public Guid MeasurementId { get; set; }
        public Guid SensorId { get; set; }
        public Sensor Sensor { get; set; } = null!;
        public double Value { get; set; }
        public DateTime MeasuredAt { get; set; }

        public Alert? Alert { get; set; }
    }
}

