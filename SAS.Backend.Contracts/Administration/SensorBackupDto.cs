using SAS.Backend.Contracts.Enums;

namespace SAS.Backend.Contracts.Administration
{
    public sealed record SensorBackupDto
    {
        public Guid SensorId { get; init; }
        public string Name { get; init; } = string.Empty;
        public SensorType SensorType { get; init; }
        public double MinValue { get; init; }
        public double MaxValue { get; init; }
        public string Status { get; init; } = "Active";
        public DateTime InstalledAt { get; init; }
        public Guid FieldId { get; init; }
    }
}
