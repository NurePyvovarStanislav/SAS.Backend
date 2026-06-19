using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;

namespace SAS.Backend.Infrastructure.Security
{
    public sealed class ResourceAccessService : IResourceAccessService
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public ResourceAccessService(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<bool> CanAccessFieldAsync(
            Guid fieldId,
            CancellationToken cancellationToken = default)
        {
            if (_userContextService.IsAdministrator())
            {
                return true;
            }

            var assignedFieldId =
                await GetCurrentUserFieldIdAsync(cancellationToken);

            return assignedFieldId.HasValue &&
                   assignedFieldId.Value == fieldId;
        }

        public async Task<bool> CanAccessSensorAsync(
            Guid sensorId,
            CancellationToken cancellationToken = default)
        {
            if (_userContextService.IsAdministrator())
            {
                return true;
            }

            var assignedFieldId =
                await GetCurrentUserFieldIdAsync(cancellationToken);

            if (!assignedFieldId.HasValue)
            {
                return false;
            }

            return await _context.Sensors
                .AsNoTracking()
                .AnyAsync(
                    sensor =>
                        sensor.SensorId == sensorId &&
                        sensor.FieldId == assignedFieldId.Value,
                    cancellationToken);
        }

        public async Task<bool> CanAccessMeasurementAsync(
            Guid measurementId,
            CancellationToken cancellationToken = default)
        {
            if (_userContextService.IsAdministrator())
            {
                return true;
            }

            var assignedFieldId =
                await GetCurrentUserFieldIdAsync(cancellationToken);

            if (!assignedFieldId.HasValue)
            {
                return false;
            }

            return await (
                from measurement in _context.Measurements.AsNoTracking()
                join sensor in _context.Sensors.AsNoTracking()
                    on measurement.SensorId equals sensor.SensorId
                where measurement.MeasurementId == measurementId &&
                      sensor.FieldId == assignedFieldId.Value
                select measurement.MeasurementId
            ).AnyAsync(cancellationToken);
        }

        public async Task<bool> CanAccessAlertAsync(
            Guid alertId,
            CancellationToken cancellationToken = default)
        {
            if (_userContextService.IsAdministrator())
            {
                return true;
            }

            var assignedFieldId =
                await GetCurrentUserFieldIdAsync(cancellationToken);

            if (!assignedFieldId.HasValue)
            {
                return false;
            }

            return await (
                from alert in _context.Alerts.AsNoTracking()
                join measurement in _context.Measurements.AsNoTracking()
                    on alert.MeasurementId equals measurement.MeasurementId
                join sensor in _context.Sensors.AsNoTracking()
                    on measurement.SensorId equals sensor.SensorId
                where alert.AlertId == alertId &&
                      sensor.FieldId == assignedFieldId.Value
                select alert.AlertId
            ).AnyAsync(cancellationToken);
        }

        private async Task<Guid?> GetCurrentUserFieldIdAsync(
            CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();

            if (!userId.HasValue)
            {
                return null;
            }

            return await _context.Users
                .AsNoTracking()
                .Where(user =>
                    user.UserId == userId.Value &&
                    user.IsActive)
                .Select(user => user.FieldId)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}