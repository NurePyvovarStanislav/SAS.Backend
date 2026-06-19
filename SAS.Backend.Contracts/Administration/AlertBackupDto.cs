using SAS.Backend.Contracts.Enums;

namespace SAS.Backend.Contracts.Administration
{
    public sealed record AlertBackupDto
    {
        public Guid AlertId { get; init; }
        public Guid MeasurementId { get; init; }
        public AlertLevel Level { get; init; }
        public string Message { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public bool IsResolved { get; init; }
        public DateTime? ResolvedAt { get; init; }
    }
}
