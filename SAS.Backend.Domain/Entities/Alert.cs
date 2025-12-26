using SAS.Backend.Domain.Enums;

namespace SAS.Backend.Domain.Entities
{
    public class Alert
    {
        public Guid AlertId { get; set; }
        public Guid MeasurementId { get; set; }
        public Measurement Measurement { get; set; } = null!;
        public AlertLevel Level { get; set; }
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsResolved { get; set; }
    }
}

