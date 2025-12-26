using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SAS.Backend.Application.Common.Interfaces;

namespace SAS.Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class BaseController : ControllerBase
    {
        private IMediator? _mediator;
        private IUserContextService? _userContextService;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
        protected IUserContextService UserContextService => _userContextService ??= HttpContext.RequestServices.GetRequiredService<IUserContextService>();

        protected Guid CurrentUserId => UserContextService.GetCurrentUserId() ?? Guid.Empty;
    }
}

