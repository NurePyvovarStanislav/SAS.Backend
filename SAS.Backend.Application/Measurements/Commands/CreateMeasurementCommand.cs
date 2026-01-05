using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Measurements;
using SAS.Backend.Domain.Entities;

namespace SAS.Backend.Application.Measurements.Commands
{
    public record CreateMeasurementCommand(Guid SensorId, double Value, DateTime MeasuredAt) : IRequest<MeasurementDto?>;

    public class CreateMeasurementCommandHandler : IRequestHandler<CreateMeasurementCommand, MeasurementDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreateMeasurementCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MeasurementDto?> Handle(CreateMeasurementCommand request, CancellationToken cancellationToken)
        {
            var sensor = await _context.Sensors
                .FirstOrDefaultAsync(s => s.SensorId == request.SensorId, cancellationToken);

            if (sensor is null)
            {
                throw new KeyNotFoundException($"Sensor {request.SensorId} not found");
            }

            var hasMin = !double.IsNaN(sensor.MinValue);
            var hasMax = !double.IsNaN(sensor.MaxValue);
            var thresholdsConfigured = hasMin || hasMax;

            var measurement = new Measurement
            {
                MeasurementId = Guid.NewGuid(),
                SensorId = request.SensorId,
                Value = request.Value,
                MeasuredAt = request.MeasuredAt
            };

            _context.Measurements.Add(measurement);

            var belowMin = hasMin && request.Value < sensor.MinValue;
            var aboveMax = hasMax && request.Value > sensor.MaxValue;
            var outOfRange = thresholdsConfigured && (belowMin || aboveMax);

            if (outOfRange)
            {
                var level = belowMin
                    ? SAS.Backend.Domain.Enums.AlertLevel.Low
                    : SAS.Backend.Domain.Enums.AlertLevel.High;

                var alert = new Alert
                {
                    AlertId = Guid.NewGuid(),
                    MeasurementId = measurement.MeasurementId,
                    Level = level,
                    Message = $"Sensor {sensor.Name} reported {request.Value} outside [{sensor.MinValue}; {sensor.MaxValue}]",
                    CreatedAt = DateTime.UtcNow,
                    IsResolved = false
                };

                _context.Alerts.Add(alert);
            }
            else if (thresholdsConfigured)
            {
                var openAlerts = await _context.Alerts
                    .Include(a => a.Measurement)
                    .Where(a => !a.IsResolved && a.Measurement.SensorId == request.SensorId)
                    .ToListAsync(cancellationToken);

                foreach (var alert in openAlerts)
                {
                    alert.IsResolved = true;
                    alert.ResolvedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<MeasurementDto>(measurement);
        }
    }
}

