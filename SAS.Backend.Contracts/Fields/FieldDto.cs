namespace SAS.Backend.Contracts.Fields
{
    public record FieldDto(Guid FieldId, string Name, string CropType, double Area, string? Location);
}

