using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;

namespace SAS.Backend.Application.Sensors.Commands
{
    public record DeleteSensorCommand(Guid SensorId) : IRequest<bool>;

    public class DeleteSensorCommandHandler : IRequestHandler<DeleteSensorCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteSensorCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteSensorCommand request, CancellationToken cancellationToken)
        {
            var sensor = await _context.Sensors
                .FirstOrDefaultAsync(s => s.SensorId == request.SensorId, cancellationToken);

            if (sensor is null)
            {
                return false;
            }

            _context.Sensors.Remove(sensor);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}

