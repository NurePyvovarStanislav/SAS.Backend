using MediatR;
using Microsoft.AspNetCore.Mvc;
using SAS.Backend.Application.Sensors.Commands;
using SAS.Backend.Application.Sensors.Queries;
using SAS.Backend.Contracts.Sensors;

namespace SAS.Backend.API.Controllers
{
    [Route("api")]
    public class SensorsController : BaseController
    {
        [HttpGet("fields/{fieldId:guid}/sensors")]
        public async Task<ActionResult<IEnumerable<SensorDto>>> GetSensorsByField(Guid fieldId, CancellationToken cancellationToken)
        {
            var sensors = await Mediator.Send(new GetSensorsByFieldQuery(fieldId), cancellationToken);
            return Ok(sensors);
        }

        [HttpPost("fields/{fieldId:guid}/sensors")]
        public async Task<ActionResult<SensorDto>> CreateSensor(Guid fieldId, [FromBody] SensorCreateDto dto, CancellationToken cancellationToken)
        {
            var created = await Mediator.Send(
                new CreateSensorCommand(fieldId, dto.Name, dto.SensorType, dto.MinValue, dto.MaxValue, dto.Status, dto.InstalledAt),
                cancellationToken);

            return created is null ? NotFound($"Field {fieldId} not found") : CreatedAtAction(nameof(GetSensorsByField), new { fieldId }, created);
        }

        [HttpPut("sensors/{id:guid}")]
        public async Task<ActionResult<SensorDto>> UpdateSensor(Guid id, [FromBody] SensorUpdateDto dto, CancellationToken cancellationToken)
        {
            var updated = await Mediator.Send(
                new UpdateSensorCommand(id, dto.Name, dto.SensorType, dto.MinValue, dto.MaxValue, dto.Status, dto.InstalledAt),
                cancellationToken);

            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("sensors/{id:guid}")]
        public async Task<IActionResult> DeleteSensor(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await Mediator.Send(new DeleteSensorCommand(id), cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}

