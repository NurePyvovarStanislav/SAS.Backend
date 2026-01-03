using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAS.Backend.Application.Auth.Commands;
using SAS.Backend.Contracts.Auth;

namespace SAS.Backend.API.Controllers
{
    public class AuthController : BaseController
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new LoginCommand(dto.Email, dto.Password), cancellationToken);
            return result is null ? Unauthorized("Invalid credentials or user is inactive") : Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshRequestDto dto, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new RefreshTokenCommand(dto.RefreshToken), cancellationToken);
            return result is null ? Unauthorized("Invalid or expired refresh token") : Ok(result);
        }
    }
}

