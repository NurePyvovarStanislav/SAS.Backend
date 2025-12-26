using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Measurements;
using SAS.Backend.Domain.Entities;

namespace SAS.Backend.Application.Measurements.Commands
{
    public record UpdateMeasurementCommand(Guid MeasurementId, double Value, DateTime MeasuredAt) : IRequest<MeasurementDto?>;

    public class UpdateMeasurementCommandHandler : IRequestHandler<UpdateMeasurementCommand, MeasurementDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdateMeasurementCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MeasurementDto?> Handle(UpdateMeasurementCommand request, CancellationToken cancellationToken)
        {
            var measurement = await _context.Measurements
                .Include(m => m.Sensor)
                .Include(m => m.Alert)
                .FirstOrDefaultAsync(m => m.MeasurementId == request.MeasurementId, cancellationToken);

            if (measurement is null) return null;

            measurement.Value = request.Value;
            measurement.MeasuredAt = request.MeasuredAt;

            var sensor = measurement.Sensor;
            var outOfRange = request.Value < sensor.MinValue || request.Value > sensor.MaxValue;

            if (outOfRange)
            {
                var level = request.Value < sensor.MinValue
                    ? SAS.Backend.Domain.Enums.AlertLevel.Low
                    : SAS.Backend.Domain.Enums.AlertLevel.High;

                if (measurement.Alert is null)
                {
                    measurement.Alert = new Alert
                    {
                        AlertId = Guid.NewGuid(),
                        MeasurementId = measurement.MeasurementId,
                        Level = level,
                        Message = $"Sensor {sensor.Name} reported {request.Value} outside [{sensor.MinValue}; {sensor.MaxValue}]",
                        CreatedAt = DateTime.UtcNow,
                        IsResolved = false
                    };
                    _context.Alerts.Add(measurement.Alert);
                }
                else
                {
                    measurement.Alert.Level = level;
                    measurement.Alert.Message = $"Sensor {sensor.Name} reported {request.Value} outside [{sensor.MinValue}; {sensor.MaxValue}]";
                    measurement.Alert.CreatedAt = DateTime.UtcNow;
                    measurement.Alert.IsResolved = false;
                }
            }
            else if (measurement.Alert is not null)
            {
                measurement.Alert.IsResolved = true;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<MeasurementDto>(measurement);
        }
    }
}

