using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAS.Backend.Application.Alerts.Commands;
using SAS.Backend.Application.Alerts.Queries;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Alerts;

namespace SAS.Backend.API.Controllers
{
    [Authorize]
    public class AlertsController : BaseController
    {
        private readonly IResourceAccessService _resourceAccessService;

        public AlertsController(
            IResourceAccessService resourceAccessService)
        {
            _resourceAccessService = resourceAccessService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlertDto>>> GetAlerts(
            [FromQuery] Guid fieldId,
            CancellationToken cancellationToken)
        {
            if (fieldId == Guid.Empty)
            {
                return BadRequest("fieldId is required");
            }

            var canAccess =
                await _resourceAccessService.CanAccessFieldAsync(
                    fieldId,
                    cancellationToken);

            if (!canAccess)
            {
                return Forbid();
            }

            var alerts = await Mediator.Send(
                new GetAlertsForFieldQuery(fieldId),
                cancellationToken);

            return Ok(alerts);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AlertDto>> GetAlert(
            Guid id,
            CancellationToken cancellationToken)
        {
            var canAccess =
                await _resourceAccessService.CanAccessAlertAsync(
                    id,
                    cancellationToken);

            if (!canAccess)
            {
                return Forbid();
            }

            var alert = await Mediator.Send(
                new GetAlertByIdQuery(id),
                cancellationToken);

            return alert is null
                ? NotFound()
                : Ok(alert);
        }

        [HttpPost("{id:guid}")]
        public async Task<IActionResult> ResolveAlert(
            Guid id,
            [FromBody] AlertResolveDto dto,
            CancellationToken cancellationToken)
        {
            var canAccess =
                await _resourceAccessService.CanAccessAlertAsync(
                    id,
                    cancellationToken);

            if (!canAccess)
            {
                return Forbid();
            }

            var updated = await Mediator.Send(
                new ResolveAlertCommand(
                    id,
                    dto.IsResolved),
                cancellationToken);

            return updated
                ? NoContent()
                : NotFound();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAlert(
            Guid id,
            CancellationToken cancellationToken)
        {
            var deleted = await Mediator.Send(
                new DeleteAlertCommand(id),
                cancellationToken);

            return deleted
                ? NoContent()
                : NotFound();
        }
    }
}