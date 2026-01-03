using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;

namespace SAS.Backend.Application.Alerts.Commands
{
    public record ResolveAlertCommand(Guid AlertId, bool IsResolved) : IRequest<bool>;

    public class ResolveAlertCommandHandler : IRequestHandler<ResolveAlertCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public ResolveAlertCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ResolveAlertCommand request, CancellationToken cancellationToken)
        {
            var alert = await _context.Alerts.FirstOrDefaultAsync(a => a.AlertId == request.AlertId, cancellationToken);
            if (alert is null) return false;

            alert.IsResolved = request.IsResolved;
            alert.ResolvedAt = request.IsResolved ? DateTime.UtcNow : null;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

