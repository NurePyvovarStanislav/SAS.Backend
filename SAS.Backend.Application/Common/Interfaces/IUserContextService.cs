namespace SAS.Backend.Application.Common.Interfaces
{
    public interface IUserContextService
    {
        Guid? GetCurrentUserId();
    }
}

