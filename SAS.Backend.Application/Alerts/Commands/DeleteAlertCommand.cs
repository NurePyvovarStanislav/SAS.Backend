using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;

namespace SAS.Backend.Application.Alerts.Commands
{
    public record DeleteAlertCommand(Guid AlertId) : IRequest<bool>;

    public class DeleteAlertCommandHandler : IRequestHandler<DeleteAlertCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteAlertCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteAlertCommand request, CancellationToken cancellationToken)
        {
            var alert = await _context.Alerts.FirstOrDefaultAsync(a => a.AlertId == request.AlertId, cancellationToken);
            if (alert is null) return false;

            _context.Alerts.Remove(alert);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

