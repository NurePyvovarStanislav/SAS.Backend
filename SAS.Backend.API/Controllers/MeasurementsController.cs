using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Application.Measurements.Commands;
using SAS.Backend.Application.Measurements.Queries;
using SAS.Backend.Contracts.Measurements;

namespace SAS.Backend.API.Controllers
{
    [Authorize]
    public class MeasurementsController : BaseController
    {
        private readonly IResourceAccessService _resourceAccessService;

        public MeasurementsController(
            IResourceAccessService resourceAccessService)
        {
            _resourceAccessService = resourceAccessService;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MeasurementDto>> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var canAccess =
                await _resourceAccessService.CanAccessMeasurementAsync(
                    id,
                    cancellationToken);

            if (!canAccess)
            {
                return Forbid();
            }

            var measurement = await Mediator.Send(
                new GetMeasurementByIdQuery(id),
                cancellationToken);

            return measurement is null
                ? NotFound()
                : Ok(measurement);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeasurementDto>>>
            GetBySensor(
                [FromQuery] Guid sensorId,
                [FromQuery] DateTime? from,
                [FromQuery] DateTime? to,
                CancellationToken cancellationToken)
        {
            if (sensorId == Guid.Empty)
            {
                return BadRequest("sensorId is required");
            }

            var canAccess =
                await _resourceAccessService.CanAccessSensorAsync(
                    sensorId,
                    cancellationToken);

            if (!canAccess)
            {
                return Forbid();
            }

            var list = await Mediator.Send(
                new GetMeasurementsBySensorQuery(
                    sensorId,
                    from,
                    to),
                cancellationToken);

            return Ok(list);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<MeasurementDto>>
            CreateMeasurement(
                [FromBody] MeasurementCreateDto dto,
                CancellationToken cancellationToken)
        {
            var created = await Mediator.Send(
                new CreateMeasurementCommand(
                    dto.SensorId,
                    dto.Value,
                    dto.MeasuredAt),
                cancellationToken);

            return created is null
                ? NotFound($"Sensor {dto.SensorId} not found")
                : Ok(created);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<MeasurementDto>>
            UpdateMeasurement(
                Guid id,
                [FromBody] MeasurementUpdateDto dto,
                CancellationToken cancellationToken)
        {
            var updated = await Mediator.Send(
                new UpdateMeasurementCommand(
                    id,
                    dto.Value,
                    dto.MeasuredAt),
                cancellationToken);

            return updated is null
                ? NotFound()
                : Ok(updated);
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteMeasurement(
            Guid id,
            CancellationToken cancellationToken)
        {
            var deleted = await Mediator.Send(
                new DeleteMeasurementCommand(id),
                cancellationToken);

            return deleted
                ? NoContent()
                : NotFound();
        }
    }
}