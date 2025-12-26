namespace SAS.Backend.Contracts.Sensors
{
    public record SensorUpdateDto
    {
        public string Name { get; init; } = string.Empty;
        public SAS.Backend.Contracts.Enums.SensorType SensorType { get; init; }
        public double MinValue { get; init; }
        public double MaxValue { get; init; }
        public string Status { get; init; } = "Active";
        public DateTime InstalledAt { get; init; }
    }
}

