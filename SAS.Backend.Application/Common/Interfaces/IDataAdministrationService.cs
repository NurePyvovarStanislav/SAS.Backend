using SAS.Backend.Contracts.Administration;

namespace SAS.Backend.Application.Common.Interfaces
{
    public interface IDataAdministrationService
    {
        Task<AdministrationSnapshotDto> CreateSnapshotAsync(
            CancellationToken cancellationToken);

        Task<ImportResultDto> ImportAsync(
            AdministrationSnapshotDto snapshot,
            CancellationToken cancellationToken);

        Task<byte[]> ExportCsvAsync(
            string entity,
            CancellationToken cancellationToken);
    }
}
