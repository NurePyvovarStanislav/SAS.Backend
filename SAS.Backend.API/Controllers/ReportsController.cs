using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAS.Backend.Application.Reports.Queries;
using SAS.Backend.Contracts.Reports;

namespace SAS.Backend.API.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ReportsController : BaseController
    {
        [HttpGet("{fieldId:guid}")]
        public async Task<ActionResult<FieldSummaryDto>> GetFieldSummary(Guid fieldId, CancellationToken cancellationToken)
        {
            var summary = await Mediator.Send(new GetFieldSummaryQuery(fieldId), cancellationToken);
            return summary is null ? NotFound() : Ok(summary);
        }
    }
}

