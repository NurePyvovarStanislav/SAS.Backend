namespace SAS.Backend.Contracts.Fields
{
    public record FieldUpdateDto
    {
        public string Name { get; init; } = string.Empty;
        public string CropType { get; init; } = string.Empty;
        public double Area { get; init; }
        public string? Location { get; init; }
    }
}

