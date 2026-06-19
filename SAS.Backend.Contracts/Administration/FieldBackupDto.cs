namespace SAS.Backend.Contracts.Administration
{
    public sealed record FieldBackupDto
    {
        public Guid FieldId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string CropType { get; init; } = string.Empty;
        public double Area { get; init; }
        public string? Location { get; init; }
    }
}
