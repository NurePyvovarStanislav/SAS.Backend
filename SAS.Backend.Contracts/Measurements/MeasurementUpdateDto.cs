namespace SAS.Backend.Contracts.Measurements
{
    public record MeasurementUpdateDto
    {
        public double Value { get; init; }
        public DateTime MeasuredAt { get; init; }
    }
}

