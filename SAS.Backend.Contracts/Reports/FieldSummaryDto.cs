namespace SAS.Backend.Contracts.Reports
{
    public record FieldSummaryDto
    {
        public Guid FieldId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string CropType { get; init; } = string.Empty;
        public double Area { get; init; }
        public string? Location { get; init; }
        public int Sensors { get; init; }
        public int Measurements { get; init; }
        public int AlertsOpen { get; init; }
        public int AlertsResolved { get; init; }
        public int UsersTotal { get; init; }
        public int UsersActive { get; init; }
    }
}

