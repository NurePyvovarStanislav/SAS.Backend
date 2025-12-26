using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;

namespace SAS.Backend.Application.Fields.Commands
{
    public record DeleteFieldCommand(Guid FieldId) : IRequest<bool>;

    public class DeleteFieldCommandHandler : IRequestHandler<DeleteFieldCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteFieldCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteFieldCommand request, CancellationToken cancellationToken)
        {
            var field = await _context.Fields
                .FirstOrDefaultAsync(f => f.FieldId == request.FieldId, cancellationToken);

            if (field is null)
            {
                return false;
            }

            _context.Fields.Remove(field);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}

