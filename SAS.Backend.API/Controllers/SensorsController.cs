using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAS.Backend.Application.Sensors.Commands;
using SAS.Backend.Application.Sensors.Queries;
using SAS.Backend.Contracts.Sensors;

namespace SAS.Backend.API.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SensorsController : BaseController
    {
        [HttpGet("{fieldId:guid}")]
        public async Task<ActionResult<IEnumerable<SensorDto>>> GetSensorsByField(Guid fieldId, CancellationToken cancellationToken)
        {
            var sensors = await Mediator.Send(new GetSensorsByFieldQuery(fieldId), cancellationToken);
            return Ok(sensors);
        }

        [HttpPost("{fieldId:guid}")]
        public async Task<ActionResult<SensorDto>> CreateSensor(Guid fieldId, [FromBody] SensorCreateDto dto, CancellationToken cancellationToken)
        {
            var created = await Mediator.Send(
                new CreateSensorCommand(fieldId, dto.Name, dto.SensorType, dto.MinValue, dto.MaxValue, dto.Status, dto.InstalledAt),
                cancellationToken);

            return created is null ? NotFound($"Field {fieldId} not found") : CreatedAtAction(nameof(GetSensorsByField), new { fieldId }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<SensorDto>> UpdateSensor(Guid id, [FromBody] SensorUpdateDto dto, CancellationToken cancellationToken)
        {
            var updated = await Mediator.Send(
                new UpdateSensorCommand(id, dto.Name, dto.SensorType, dto.MinValue, dto.MaxValue, dto.Status, dto.InstalledAt),
                cancellationToken);

            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSensor(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await Mediator.Send(new DeleteSensorCommand(id), cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}

