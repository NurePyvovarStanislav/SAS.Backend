using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Administration;

namespace SAS.Backend.API.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministrationController : BaseController
    {
        private const long MaxImportFileSize = 10 * 1024 * 1024;

        [HttpGet]
        public async Task<IActionResult> CreateBackup(CancellationToken cancellationToken)
        {
            var service = HttpContext.RequestServices.GetRequiredService<IDataAdministrationService>();
            var snapshot = await service.CreateSnapshotAsync(cancellationToken);

            var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var fileName = $"sas-backup-{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}.json";
            return File(
                Encoding.UTF8.GetBytes(json),
                "application/json",
                fileName);
        }

        [HttpGet]
        public async Task<IActionResult> ExportData(
            [FromQuery] string format,
            [FromQuery] string entity,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(format) || string.IsNullOrWhiteSpace(entity))
            {
                return BadRequest("format and entity are required.");
            }

            var normalizedFormat = format.ToLowerInvariant();
            var normalizedEntity = entity.ToLowerInvariant();

            var service = HttpContext.RequestServices.GetRequiredService<IDataAdministrationService>();

            if (normalizedFormat == "json")
            {
                var snapshot = await service.CreateSnapshotAsync(cancellationToken);
                var filtered = FilterSnapshot(snapshot, normalizedEntity);
                var json = JsonSerializer.Serialize(filtered, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                var fileName = $"sas-export-{normalizedEntity}-{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}.json";
                return File(Encoding.UTF8.GetBytes(json), "application/json", fileName);
            }

            if (normalizedFormat == "csv")
            {
                if (normalizedEntity == "all")
                {
                    return BadRequest("CSV export is not supported for entity=all.");
                }

                try
                {
                    var csvBytes = await service.ExportCsvAsync(normalizedEntity, cancellationToken);
                    var fileName = $"sas-export-{normalizedEntity}-{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}.csv";
                    return File(csvBytes, "text/csv", fileName);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return BadRequest("Unsupported format. Use json or csv.");
        }

        [HttpPost]
        public async Task<ActionResult<ImportResultDto>> ImportData(
            IFormFile file,
            CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest("File is empty.");
            }

            if (file.Length > MaxImportFileSize)
            {
                return BadRequest("File exceeds the 10 MB limit.");
            }

            if (!file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only .json files are supported.");
            }

            AdministrationSnapshotDto? snapshot;

            try
            {
                await using var stream = file.OpenReadStream();
                snapshot = await JsonSerializer.DeserializeAsync<AdministrationSnapshotDto>(
                    stream,
                    cancellationToken: cancellationToken);
            }
            catch (JsonException)
            {
                return BadRequest("Invalid JSON file.");
            }

            if (snapshot is null)
            {
                return BadRequest("Invalid JSON file.");
            }

            var service = HttpContext.RequestServices.GetRequiredService<IDataAdministrationService>();
            var result = await service.ImportAsync(snapshot, cancellationToken);

            return Ok(result);
        }

        private static AdministrationSnapshotDto FilterSnapshot(
            AdministrationSnapshotDto snapshot,
            string entity)
        {
            return entity switch
            {
                "all" => snapshot,
                "users" => snapshot with { Fields = Array.Empty<FieldBackupDto>(), Sensors = Array.Empty<SensorBackupDto>(), Measurements = Array.Empty<MeasurementBackupDto>(), Alerts = Array.Empty<AlertBackupDto>() },
                "fields" => snapshot with { Users = Array.Empty<UserBackupDto>(), Sensors = Array.Empty<SensorBackupDto>(), Measurements = Array.Empty<MeasurementBackupDto>(), Alerts = Array.Empty<AlertBackupDto>() },
                "sensors" => snapshot with { Users = Array.Empty<UserBackupDto>(), Fields = Array.Empty<FieldBackupDto>(), Measurements = Array.Empty<MeasurementBackupDto>(), Alerts = Array.Empty<AlertBackupDto>() },
                "measurements" => snapshot with { Users = Array.Empty<UserBackupDto>(), Fields = Array.Empty<FieldBackupDto>(), Sensors = Array.Empty<SensorBackupDto>(), Alerts = Array.Empty<AlertBackupDto>() },
                "alerts" => snapshot with { Users = Array.Empty<UserBackupDto>(), Fields = Array.Empty<FieldBackupDto>(), Sensors = Array.Empty<SensorBackupDto>(), Measurements = Array.Empty<MeasurementBackupDto>() },
                _ => snapshot with
                {
                    Users = Array.Empty<UserBackupDto>(),
                    Fields = Array.Empty<FieldBackupDto>(),
                    Sensors = Array.Empty<SensorBackupDto>(),
                    Measurements = Array.Empty<MeasurementBackupDto>(),
                    Alerts = Array.Empty<AlertBackupDto>()
                }
            };
        }
    }
}
