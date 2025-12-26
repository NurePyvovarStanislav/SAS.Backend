namespace SAS.Backend.Domain.Entities
{
    public class Field
    {
        public Guid FieldId { get; set; }
        public string Name { get; set; } = null!;
        public string CropType { get; set; } = null!;
        public double Area { get; set; }
        public string? Location { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();
    }
}

