namespace SAS.Backend.Contracts.Fields
{
    public record FieldCreateDto
    {
        public string Name { get; init; } = string.Empty;
        public string CropType { get; init; } = string.Empty;
        public double Area { get; init; }
        public string? Location { get; init; }
        public Guid UserId { get; init; }
    }
}

