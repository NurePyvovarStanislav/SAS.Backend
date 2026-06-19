namespace SAS.Backend.Contracts.Administration
{
    public sealed record AdministrationSnapshotDto
    {
        public string SchemaVersion { get; init; } = "1.0";
        public DateTime CreatedAtUtc { get; init; }

        public IReadOnlyList<UserBackupDto> Users { get; init; }
            = Array.Empty<UserBackupDto>();

        public IReadOnlyList<FieldBackupDto> Fields { get; init; }
            = Array.Empty<FieldBackupDto>();

        public IReadOnlyList<SensorBackupDto> Sensors { get; init; }
            = Array.Empty<SensorBackupDto>();

        public IReadOnlyList<MeasurementBackupDto> Measurements { get; init; }
            = Array.Empty<MeasurementBackupDto>();

        public IReadOnlyList<AlertBackupDto> Alerts { get; init; }
            = Array.Empty<AlertBackupDto>();

        public Dictionary<string, string> Settings { get; init; } = new();
    }
}
