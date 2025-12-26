using MediatR;
using Microsoft.AspNetCore.Mvc;
using SAS.Backend.Application.Alerts.Queries;
using SAS.Backend.Contracts.Alerts;

namespace SAS.Backend.API.Controllers
{
    public class AlertsController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlertDto>>> GetAlerts([FromQuery] Guid fieldId, CancellationToken cancellationToken)
        {
            if (fieldId == Guid.Empty)
            {
                return BadRequest("fieldId is required");
            }

            var alerts = await Mediator.Send(new GetAlertsForFieldQuery(fieldId), cancellationToken);
            return Ok(alerts);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AlertDto>> GetAlert(Guid id, CancellationToken cancellationToken)
        {
            var alert = await Mediator.Send(new SAS.Backend.Application.Alerts.Queries.GetAlertByIdQuery(id), cancellationToken);
            return alert is null ? NotFound() : Ok(alert);
        }

        [HttpPost("{id:guid}")]
        public async Task<IActionResult> ResolveAlert(Guid id, [FromBody] AlertResolveDto dto, CancellationToken cancellationToken)
        {
            var updated = await Mediator.Send(new SAS.Backend.Application.Alerts.Commands.ResolveAlertCommand(id, dto.IsResolved), cancellationToken);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAlert(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await Mediator.Send(new SAS.Backend.Application.Alerts.Commands.DeleteAlertCommand(id), cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}

