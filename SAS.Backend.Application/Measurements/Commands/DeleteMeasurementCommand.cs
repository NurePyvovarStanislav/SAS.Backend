using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;

namespace SAS.Backend.Application.Measurements.Commands
{
    public record DeleteMeasurementCommand(Guid MeasurementId) : IRequest<bool>;

    public class DeleteMeasurementCommandHandler : IRequestHandler<DeleteMeasurementCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteMeasurementCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteMeasurementCommand request, CancellationToken cancellationToken)
        {
            var measurement = await _context.Measurements.FirstOrDefaultAsync(m => m.MeasurementId == request.MeasurementId, cancellationToken);
            if (measurement is null) return false;

            _context.Measurements.Remove(measurement);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

