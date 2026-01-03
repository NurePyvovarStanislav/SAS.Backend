namespace SAS.Backend.Contracts.Alerts
{
    public record AlertDto(Guid AlertId, Guid MeasurementId, SAS.Backend.Contracts.Enums.AlertLevel Level, string Message, DateTime CreatedAt, bool IsResolved, DateTime? ResolvedAt);
}

