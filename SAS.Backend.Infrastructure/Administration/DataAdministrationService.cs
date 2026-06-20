using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Administration;
using SAS.Backend.Contracts.Enums;
using SAS.Backend.Domain.Entities;
using SAS.Backend.Infrastructure.Persistence;

namespace SAS.Backend.Infrastructure.Administration
{
    public sealed class DataAdministrationService : IDataAdministrationService
    {
        private const string SupportedSchemaVersion = "1.0";
        private readonly ApplicationDbContext _context;

        public DataAdministrationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdministrationSnapshotDto> CreateSnapshotAsync(
            CancellationToken cancellationToken)
        {
            // Сначала загружаем сущности из БД без преобразования enum в SQL.
            var userEntities = await _context.Users
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var fieldEntities = await _context.Fields
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var sensorEntities = await _context.Sensors
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var measurementEntities = await _context.Measurements
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var alertEntities = await _context.Alerts
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Преобразования выполняются уже в памяти,
            // поэтому PostgreSQL не пытается преобразовать названия enum.
            var users = userEntities
                .Select(user => new UserBackupDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = (UserRole)(int)user.Role,
                    Phone = user.Phone,
                    FieldId = user.FieldId,
                    IsActive = user.IsActive
                })
                .ToList();

            var fields = fieldEntities
                .Select(field => new FieldBackupDto
                {
                    FieldId = field.FieldId,
                    Name = field.Name,
                    CropType = field.CropType,
                    Area = field.Area,
                    Location = field.Location
                })
                .ToList();

            var sensors = sensorEntities
                .Select(sensor => new SensorBackupDto
                {
                    SensorId = sensor.SensorId,
                    Name = sensor.Name,
                    SensorType = (SensorType)(int)sensor.SensorType,
                    MinValue = sensor.MinValue,
                    MaxValue = sensor.MaxValue,
                    Status = sensor.Status,
                    InstalledAt = sensor.InstalledAt,
                    FieldId = sensor.FieldId
                })
                .ToList();

            var measurements = measurementEntities
                .Select(measurement => new MeasurementBackupDto
                {
                    MeasurementId = measurement.MeasurementId,
                    SensorId = measurement.SensorId,
                    Value = measurement.Value,
                    MeasuredAt = measurement.MeasuredAt
                })
                .ToList();

            var alerts = alertEntities
                .Select(alert => new AlertBackupDto
                {
                    AlertId = alert.AlertId,
                    MeasurementId = alert.MeasurementId,
                    Level = (AlertLevel)(int)alert.Level,
                    Message = alert.Message,
                    CreatedAt = alert.CreatedAt,
                    IsResolved = alert.IsResolved,
                    ResolvedAt = alert.ResolvedAt
                })
                .ToList();

            return new AdministrationSnapshotDto
            {
                SchemaVersion = SupportedSchemaVersion,
                CreatedAtUtc = DateTime.UtcNow,
                Users = users,
                Fields = fields,
                Sensors = sensors,
                Measurements = measurements,
                Alerts = alerts,
                Settings = new Dictionary<string, string>()
            };
        }

        public async Task<ImportResultDto> ImportAsync(
            AdministrationSnapshotDto snapshot,
            CancellationToken cancellationToken)
        {
            if (snapshot.SchemaVersion != SupportedSchemaVersion)
            {
                return new ImportResultDto(0, 0, 0, new[]
                {
                    $"Unsupported schema version: {snapshot.SchemaVersion}. Expected {SupportedSchemaVersion}."
                });
            }

            var created = 0;
            var updated = 0;
            var skipped = 0;
            var warnings = new List<string>();

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                foreach (var fieldDto in snapshot.Fields)
                {
                    try
                    {
                        var existing = await _context.Fields
                            .FirstOrDefaultAsync(f => f.FieldId == fieldDto.FieldId, cancellationToken);

                        if (existing is null)
                        {
                            _context.Fields.Add(new Field
                            {
                                FieldId = fieldDto.FieldId,
                                Name = fieldDto.Name,
                                CropType = fieldDto.CropType,
                                Area = fieldDto.Area,
                                Location = fieldDto.Location
                            });
                            created++;
                        }
                        else
                        {
                            existing.Name = fieldDto.Name;
                            existing.CropType = fieldDto.CropType;
                            existing.Area = fieldDto.Area;
                            existing.Location = fieldDto.Location;
                            updated++;
                        }
                    }
                    catch (Exception ex)
                    {
                        skipped++;
                        warnings.Add($"Field {fieldDto.FieldId}: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                foreach (var userDto in snapshot.Users)
                {
                    try
                    {
                        var existing = await _context.Users
                            .FirstOrDefaultAsync(u => u.UserId == userDto.UserId, cancellationToken);

                        if (existing is null)
                        {
                            skipped++;
                            warnings.Add($"User {userDto.Email}: skipped — new users cannot be imported without a password.");
                            continue;
                        }

                        existing.Email = userDto.Email;
                        existing.FullName = userDto.FullName;
                        existing.Role = Enum.Parse<Domain.Enums.UserRole>(userDto.Role.ToString());
                        existing.Phone = userDto.Phone;
                        existing.FieldId = userDto.FieldId;
                        existing.IsActive = userDto.IsActive;
                        updated++;
                    }
                    catch (Exception ex)
                    {
                        skipped++;
                        warnings.Add($"User {userDto.UserId}: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                foreach (var sensorDto in snapshot.Sensors)
                {
                    try
                    {
                        var fieldExists = await _context.Fields
                            .AnyAsync(f => f.FieldId == sensorDto.FieldId, cancellationToken);

                        if (!fieldExists)
                        {
                            skipped++;
                            warnings.Add($"Sensor {sensorDto.SensorId}: field {sensorDto.FieldId} not found.");
                            continue;
                        }

                        var existing = await _context.Sensors
                            .FirstOrDefaultAsync(s => s.SensorId == sensorDto.SensorId, cancellationToken);

                        if (existing is null)
                        {
                            _context.Sensors.Add(new Sensor
                            {
                                SensorId = sensorDto.SensorId,
                                Name = sensorDto.Name,
                                SensorType = Enum.Parse<Domain.Enums.SensorType>(sensorDto.SensorType.ToString()),
                                MinValue = sensorDto.MinValue,
                                MaxValue = sensorDto.MaxValue,
                                Status = sensorDto.Status,
                                InstalledAt = sensorDto.InstalledAt,
                                FieldId = sensorDto.FieldId
                            });
                            created++;
                        }
                        else
                        {
                            existing.Name = sensorDto.Name;
                            existing.SensorType = Enum.Parse<Domain.Enums.SensorType>(sensorDto.SensorType.ToString());
                            existing.MinValue = sensorDto.MinValue;
                            existing.MaxValue = sensorDto.MaxValue;
                            existing.Status = sensorDto.Status;
                            existing.InstalledAt = sensorDto.InstalledAt;
                            existing.FieldId = sensorDto.FieldId;
                            updated++;
                        }
                    }
                    catch (Exception ex)
                    {
                        skipped++;
                        warnings.Add($"Sensor {sensorDto.SensorId}: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                foreach (var measurementDto in snapshot.Measurements)
                {
                    try
                    {
                        var sensorExists = await _context.Sensors
                            .AnyAsync(s => s.SensorId == measurementDto.SensorId, cancellationToken);

                        if (!sensorExists)
                        {
                            skipped++;
                            warnings.Add($"Measurement {measurementDto.MeasurementId}: sensor {measurementDto.SensorId} not found.");
                            continue;
                        }

                        var existing = await _context.Measurements
                            .FirstOrDefaultAsync(m => m.MeasurementId == measurementDto.MeasurementId, cancellationToken);

                        if (existing is null)
                        {
                            _context.Measurements.Add(new Measurement
                            {
                                MeasurementId = measurementDto.MeasurementId,
                                SensorId = measurementDto.SensorId,
                                Value = measurementDto.Value,
                                MeasuredAt = measurementDto.MeasuredAt
                            });
                            created++;
                        }
                        else
                        {
                            existing.SensorId = measurementDto.SensorId;
                            existing.Value = measurementDto.Value;
                            existing.MeasuredAt = measurementDto.MeasuredAt;
                            updated++;
                        }
                    }
                    catch (Exception ex)
                    {
                        skipped++;
                        warnings.Add($"Measurement {measurementDto.MeasurementId}: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                foreach (var alertDto in snapshot.Alerts)
                {
                    try
                    {
                        var measurementExists = await _context.Measurements
                            .AnyAsync(m => m.MeasurementId == alertDto.MeasurementId, cancellationToken);

                        if (!measurementExists)
                        {
                            skipped++;
                            warnings.Add($"Alert {alertDto.AlertId}: measurement {alertDto.MeasurementId} not found.");
                            continue;
                        }

                        var existing = await _context.Alerts
                            .FirstOrDefaultAsync(a => a.AlertId == alertDto.AlertId, cancellationToken);

                        if (existing is null)
                        {
                            _context.Alerts.Add(new Alert
                            {
                                AlertId = alertDto.AlertId,
                                MeasurementId = alertDto.MeasurementId,
                                Level = Enum.Parse<Domain.Enums.AlertLevel>(alertDto.Level.ToString()),
                                Message = alertDto.Message,
                                CreatedAt = alertDto.CreatedAt,
                                IsResolved = alertDto.IsResolved,
                                ResolvedAt = alertDto.ResolvedAt
                            });
                            created++;
                        }
                        else
                        {
                            existing.MeasurementId = alertDto.MeasurementId;
                            existing.Level = Enum.Parse<Domain.Enums.AlertLevel>(alertDto.Level.ToString());
                            existing.Message = alertDto.Message;
                            existing.CreatedAt = alertDto.CreatedAt;
                            existing.IsResolved = alertDto.IsResolved;
                            existing.ResolvedAt = alertDto.ResolvedAt;
                            updated++;
                        }
                    }
                    catch (Exception ex)
                    {
                        skipped++;
                        warnings.Add($"Alert {alertDto.AlertId}: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                warnings.Add($"Import failed: {ex.Message}");
            }

            return new ImportResultDto(created, updated, skipped, warnings);
        }

        public async Task<byte[]> ExportCsvAsync(
            string entity,
            CancellationToken cancellationToken)
        {
            var normalized = entity.ToLowerInvariant();

            return normalized switch
            {
                "users" => await ExportUsersCsvAsync(cancellationToken),
                "fields" => await ExportFieldsCsvAsync(cancellationToken),
                "sensors" => await ExportSensorsCsvAsync(cancellationToken),
                "measurements" => await ExportMeasurementsCsvAsync(cancellationToken),
                "alerts" => await ExportAlertsCsvAsync(cancellationToken),
                _ => throw new ArgumentException($"Unsupported entity: {entity}")
            };
        }

        private async Task<byte[]> ExportUsersCsvAsync(CancellationToken cancellationToken)
        {
            var rows = await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Email)
                .Select(u => new[]
                {
                    u.UserId.ToString(),
                    u.Email,
                    u.FullName,
                    u.Role.ToString(),
                    u.Phone ?? string.Empty,
                    u.FieldId == null ? string.Empty : u.FieldId.ToString(),
                    u.IsActive.ToString()
                })
                .ToListAsync(cancellationToken);

            return BuildCsv(
                new[] { "UserId", "Email", "FullName", "Role", "Phone", "FieldId", "IsActive" },
                rows);
        }

        private async Task<byte[]> ExportFieldsCsvAsync(CancellationToken cancellationToken)
        {
            var rows = await _context.Fields
                .AsNoTracking()
                .OrderBy(f => f.Name)
                .Select(f => new[]
                {
                    f.FieldId.ToString(),
                    f.Name,
                    f.CropType,
                    f.Area.ToString(CultureInfo.InvariantCulture),
                    f.Location ?? string.Empty
                })
                .ToListAsync(cancellationToken);

            return BuildCsv(
                new[] { "FieldId", "Name", "CropType", "Area", "Location" },
                rows);
        }

        private async Task<byte[]> ExportSensorsCsvAsync(CancellationToken cancellationToken)
        {
            var rows = await _context.Sensors
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .Select(s => new[]
                {
                    s.SensorId.ToString(),
                    s.Name,
                    s.SensorType.ToString(),
                    s.MinValue.ToString(CultureInfo.InvariantCulture),
                    s.MaxValue.ToString(CultureInfo.InvariantCulture),
                    s.Status,
                    s.InstalledAt.ToString("O", CultureInfo.InvariantCulture),
                    s.FieldId.ToString()
                })
                .ToListAsync(cancellationToken);

            return BuildCsv(
                new[] { "SensorId", "Name", "SensorType", "MinValue", "MaxValue", "Status", "InstalledAt", "FieldId" },
                rows);
        }

        private async Task<byte[]> ExportMeasurementsCsvAsync(CancellationToken cancellationToken)
        {
            var rows = await _context.Measurements
                .AsNoTracking()
                .OrderBy(m => m.MeasuredAt)
                .Select(m => new[]
                {
                    m.MeasurementId.ToString(),
                    m.SensorId.ToString(),
                    m.Value.ToString(CultureInfo.InvariantCulture),
                    m.MeasuredAt.ToString("O", CultureInfo.InvariantCulture)
                })
                .ToListAsync(cancellationToken);

            return BuildCsv(
                new[] { "MeasurementId", "SensorId", "Value", "MeasuredAt" },
                rows);
        }

        private async Task<byte[]> ExportAlertsCsvAsync(CancellationToken cancellationToken)
        {
            var rows = await _context.Alerts
                .AsNoTracking()
                .OrderBy(a => a.CreatedAt)
                .Select(a => new[]
                {
                    a.AlertId.ToString(),
                    a.MeasurementId.ToString(),
                    a.Level.ToString(),
                    a.Message,
                    a.CreatedAt.ToString("O", CultureInfo.InvariantCulture),
                    a.IsResolved.ToString(),
                    a.ResolvedAt == null ? string.Empty : a.ResolvedAt.Value.ToString("O", CultureInfo.InvariantCulture)
                })
                .ToListAsync(cancellationToken);

            return BuildCsv(
                new[] { "AlertId", "MeasurementId", "Level", "Message", "CreatedAt", "IsResolved", "ResolvedAt" },
                rows);
        }

        private static byte[] BuildCsv(string[] headers, IReadOnlyList<string[]> rows)
        {
            var builder = new StringBuilder();
            builder.AppendLine(string.Join(",", headers.Select(EscapeCsv)));

            foreach (var row in rows)
            {
                builder.AppendLine(string.Join(",", row.Select(EscapeCsv)));
            }

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        private static string EscapeCsv(string value)
        {
            if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }
    }
}
