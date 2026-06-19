using System.Security.Claims;
using SAS.Backend.Application.Common.Interfaces;

namespace SAS.Backend.API.Services
{
    public sealed class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var idValue = user?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            return Guid.TryParse(idValue, out var id)
                ? id
                : null;
        }

        public bool IsAdministrator()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            return user?.Identity?.IsAuthenticated == true &&
                   user.IsInRole("Administrator");
        }
    }
}