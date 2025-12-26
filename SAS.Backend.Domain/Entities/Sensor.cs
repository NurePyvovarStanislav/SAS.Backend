
using SAS.Backend.Domain.Enums;

namespace SAS.Backend.Domain.Entities
{
    public class Sensor
    {
        public Guid SensorId { get; set; }
        public string Name { get; set; } = null!;
        public SensorType SensorType { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime InstalledAt { get; set; }

        public Guid FieldId { get; set; }
        public Field Field { get; set; } = null!;

        public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
    }
}
