using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Reports;

namespace SAS.Backend.Application.Reports.Queries
{
    public record GetFieldSummaryQuery(Guid FieldId) : IRequest<FieldSummaryDto?>;

    public class GetFieldSummaryQueryHandler : IRequestHandler<GetFieldSummaryQuery, FieldSummaryDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetFieldSummaryQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FieldSummaryDto?> Handle(GetFieldSummaryQuery request, CancellationToken cancellationToken)
        {
            var field = await _context.Fields.AsNoTracking().FirstOrDefaultAsync(f => f.FieldId == request.FieldId, cancellationToken);
            if (field is null)
            {
                return null;
            }

            var sensorsCount = await _context.Sensors.CountAsync(s => s.FieldId == request.FieldId, cancellationToken);

            var measurementsCount = await _context.Measurements
                .Where(m => _context.Sensors.Any(s => s.SensorId == m.SensorId && s.FieldId == request.FieldId))
                .CountAsync(cancellationToken);

            var alertsQuery = _context.Alerts
                .Include(a => a.Measurement)
                .ThenInclude(m => m.Sensor)
                .Where(a => a.Measurement.Sensor.FieldId == request.FieldId);

            var alertsOpen = await alertsQuery
                .Where(a => !a.IsResolved)
                .CountAsync(cancellationToken);

            var alertsResolved = await alertsQuery
                .Where(a => a.IsResolved)
                .CountAsync(cancellationToken);

            var usersTotal = await _context.Users.CountAsync(u => u.FieldId == request.FieldId, cancellationToken);
            var usersActive = await _context.Users.CountAsync(u => u.FieldId == request.FieldId && u.IsActive, cancellationToken);

            return new FieldSummaryDto
            {
                FieldId = field.FieldId,
                Name = field.Name,
                CropType = field.CropType,
                Area = field.Area,
                Location = field.Location,
                Sensors = sensorsCount,
                Measurements = measurementsCount,
                AlertsOpen = alertsOpen,
                AlertsResolved = alertsResolved,
                UsersTotal = usersTotal,
                UsersActive = usersActive
            };
        }
    }
}

