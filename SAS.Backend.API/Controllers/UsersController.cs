using Microsoft.AspNetCore.Mvc;
using SAS.Backend.Application.Users.Commands;
using SAS.Backend.Application.Users.Queries;
using SAS.Backend.Contracts.Users;

namespace SAS.Backend.API.Controllers
{
    public class UsersController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetUsersQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id, CancellationToken cancellationToken)
        {
            var user = await Mediator.Send(new GetUserByIdQuery(id), cancellationToken);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserCreateDto dto, CancellationToken cancellationToken)
        {
            var created = await Mediator.Send(new CreateUserCommand(dto.Email, dto.Password, dto.FullName, dto.Role, dto.Phone, dto.FieldId), cancellationToken);
            return created is null ? Conflict("Email already exists") : CreatedAtAction(nameof(GetUser), new { id = created.UserId }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UserUpdateDto dto, CancellationToken cancellationToken)
        {
            var updated = await Mediator.Send(new UpdateUserCommand(id, dto.Email, dto.FullName, dto.Role, dto.Phone, dto.Password, dto.FieldId), cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await Mediator.Send(new DeleteUserCommand(id), cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
    }
}

