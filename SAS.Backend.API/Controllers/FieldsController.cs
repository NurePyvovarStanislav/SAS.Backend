using MediatR;
using Microsoft.AspNetCore.Mvc;
using SAS.Backend.Application.Fields.Commands;
using SAS.Backend.Application.Fields.Queries;
using SAS.Backend.Contracts.Fields;

namespace SAS.Backend.API.Controllers
{
    public class FieldsController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FieldDto>>> GetFields(CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetFieldsQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FieldDto>> GetField(Guid id, CancellationToken cancellationToken)
        {
            var field = await Mediator.Send(new GetFieldByIdQuery(id), cancellationToken);
            return field is null ? NotFound() : Ok(field);
        }

        [HttpPost]
        public async Task<ActionResult<FieldDto>> CreateField([FromBody] FieldCreateDto dto, CancellationToken cancellationToken)
        {
            var created = await Mediator.Send(new CreateFieldCommand(dto.Name, dto.CropType, dto.Area, dto.Location, dto.UserId), cancellationToken);
            return CreatedAtAction(nameof(GetField), new { id = created.FieldId }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FieldDto>> UpdateField(Guid id, [FromBody] FieldUpdateDto dto, CancellationToken cancellationToken)
        {
            var updated = await Mediator.Send(new UpdateFieldCommand(id, dto.Name, dto.CropType, dto.Area, dto.Location), cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteField(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await Mediator.Send(new DeleteFieldCommand(id), cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}

