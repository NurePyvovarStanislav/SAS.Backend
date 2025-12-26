namespace SAS.Backend.Contracts.Measurements
{
    public record MeasurementDto(Guid MeasurementId, Guid SensorId, double Value, DateTime MeasuredAt);
}

