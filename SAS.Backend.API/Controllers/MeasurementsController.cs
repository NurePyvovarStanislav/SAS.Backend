using MediatR;
using Microsoft.AspNetCore.Mvc;
using SAS.Backend.Application.Measurements.Commands;
using SAS.Backend.Contracts.Measurements;

namespace SAS.Backend.API.Controllers
{
    public class MeasurementsController : BaseController
    {
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MeasurementDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var measurement = await Mediator.Send(new SAS.Backend.Application.Measurements.Queries.GetMeasurementByIdQuery(id), cancellationToken);
            return measurement is null ? NotFound() : Ok(measurement);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeasurementDto>>> GetBySensor([FromQuery] Guid sensorId, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
        {
            if (sensorId == Guid.Empty) return BadRequest("sensorId is required");
            var list = await Mediator.Send(new SAS.Backend.Application.Measurements.Queries.GetMeasurementsBySensorQuery(sensorId, from, to), cancellationToken);
            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult<MeasurementDto>> CreateMeasurement([FromBody] MeasurementCreateDto dto, CancellationToken cancellationToken)
        {
            var created = await Mediator.Send(new CreateMeasurementCommand(dto.SensorId, dto.Value, dto.MeasuredAt), cancellationToken);
            return created is null ? NotFound($"Sensor {dto.SensorId} not found") : Ok(created);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<MeasurementDto>> UpdateMeasurement(Guid id, [FromBody] MeasurementUpdateDto dto, CancellationToken cancellationToken)
        {
            var updated = await Mediator.Send(new SAS.Backend.Application.Measurements.Commands.UpdateMeasurementCommand(id, dto.Value, dto.MeasuredAt), cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteMeasurement(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await Mediator.Send(new SAS.Backend.Application.Measurements.Commands.DeleteMeasurementCommand(id), cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}

