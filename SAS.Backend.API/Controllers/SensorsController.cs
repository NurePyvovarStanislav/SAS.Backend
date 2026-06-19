using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Application.Sensors.Commands;
using SAS.Backend.Application.Sensors.Queries;
using SAS.Backend.Contracts.Sensors;

namespace SAS.Backend.API.Controllers
{
    [Authorize]
    public class SensorsController : BaseController
    {
        private readonly IResourceAccessService _resourceAccessService;

        public SensorsController(
            IResourceAccessService resourceAccessService)
        {
            _resourceAccessService = resourceAccessService;
        }

        [HttpGet("{fieldId:guid}")]
        public async Task<ActionResult<IEnumerable<SensorDto>>>
            GetSensorsByField(
                Guid fieldId,
                CancellationToken cancellationToken)
        {
            var canAccess =
                await _resourceAccessService.CanAccessFieldAsync(
                    fieldId,
                    cancellationToken);

            if (!canAccess)
            {
                return Forbid();
            }

            var sensors = await Mediator.Send(
                new GetSensorsByFieldQuery(fieldId),
                cancellationToken);

            return Ok(sensors);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("{fieldId:guid}")]
        public async Task<ActionResult<SensorDto>> CreateSensor(
            Guid fieldId,
            [FromBody] SensorCreateDto dto,
            CancellationToken cancellationToken)
        {
            var created = await Mediator.Send(
                new CreateSensorCommand(
                    fieldId,
                    dto.Name,
                    dto.SensorType,
                    dto.MinValue,
                    dto.MaxValue,
                    dto.Status,
                    dto.InstalledAt),
                cancellationToken);

            if (created is null)
            {
                return NotFound($"Field {fieldId} not found");
            }

            return CreatedAtAction(
                nameof(GetSensorsByField),
                new { fieldId },
                created);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<SensorDto>> UpdateSensor(
            Guid id,
            [FromBody] SensorUpdateDto dto,
            CancellationToken cancellationToken)
        {
            var updated = await Mediator.Send(
                new UpdateSensorCommand(
                    id,
                    dto.Name,
                    dto.SensorType,
                    dto.MinValue,
                    dto.MaxValue,
                    dto.Status,
                    dto.InstalledAt),
                cancellationToken);

            return updated is null
                ? NotFound()
                : Ok(updated);
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSensor(
            Guid id,
            CancellationToken cancellationToken)
        {
            var deleted = await Mediator.Send(
                new DeleteSensorCommand(id),
                cancellationToken);

            return deleted
                ? NoContent()
                : NotFound();
        }
    }
}