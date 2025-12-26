using MediatR;
using Microsoft.AspNetCore.Mvc;
using SAS.Backend.Application.Measurements.Commands;
using SAS.Backend.Contracts.Measurements;

namespace SAS.Backend.API.Controllers
{
    [Route("api/[controller]")]
    public class MeasurementsController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<MeasurementDto>> CreateMeasurement([FromBody] MeasurementCreateDto dto, CancellationToken cancellationToken)
        {
            var created = await Mediator.Send(new CreateMeasurementCommand(dto.SensorId, dto.Value, dto.MeasuredAt), cancellationToken);
            return created is null ? NotFound($"Sensor {dto.SensorId} not found") : Ok(created);
        }
    }
}

