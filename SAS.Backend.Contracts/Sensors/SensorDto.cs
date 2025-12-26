namespace SAS.Backend.Contracts.Sensors
{
    public record SensorDto(Guid SensorId, string Name, SAS.Backend.Contracts.Enums.SensorType SensorType, double MinValue, double MaxValue, string Status, DateTime InstalledAt, Guid FieldId);
}

