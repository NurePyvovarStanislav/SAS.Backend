using MediatR;
using Microsoft.AspNetCore.Mvc;
using SAS.Backend.Application.Alerts.Queries;
using SAS.Backend.Contracts.Alerts;

namespace SAS.Backend.API.Controllers
{
    [Route("api/[controller]")]
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
    }
}

