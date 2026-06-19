namespace SAS.Backend.Contracts.Administration
{
    public sealed record MeasurementBackupDto
    {
        public Guid MeasurementId { get; init; }
        public Guid SensorId { get; init; }
        public double Value { get; init; }
        public DateTime MeasuredAt { get; init; }
    }
}
