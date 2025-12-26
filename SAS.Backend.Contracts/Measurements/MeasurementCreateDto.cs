namespace SAS.Backend.Contracts.Measurements
{
    public record MeasurementCreateDto
    {
        public Guid SensorId { get; init; }
        public double Value { get; init; }
        public DateTime MeasuredAt { get; init; }
    }
}

