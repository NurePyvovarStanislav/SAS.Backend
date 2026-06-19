namespace SAS.Backend.Application.Common.Interfaces
{
    public interface IResourceAccessService
    {
        Task<bool> CanAccessFieldAsync(
            Guid fieldId,
            CancellationToken cancellationToken = default);

        Task<bool> CanAccessSensorAsync(
            Guid sensorId,
            CancellationToken cancellationToken = default);

        Task<bool> CanAccessMeasurementAsync(
            Guid measurementId,
            CancellationToken cancellationToken = default);

        Task<bool> CanAccessAlertAsync(
            Guid alertId,
            CancellationToken cancellationToken = default);
    }
}