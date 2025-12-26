using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Sensors;

namespace SAS.Backend.Application.Sensors.Commands
{
    public record UpdateSensorCommand(Guid SensorId, string Name, SAS.Backend.Contracts.Enums.SensorType SensorType, double MinValue, double MaxValue, string Status, DateTime InstalledAt) : IRequest<SensorDto?>;

    public class UpdateSensorCommandHandler : IRequestHandler<UpdateSensorCommand, SensorDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdateSensorCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SensorDto?> Handle(UpdateSensorCommand request, CancellationToken cancellationToken)
        {
            var sensor = await _context.Sensors
                .FirstOrDefaultAsync(s => s.SensorId == request.SensorId, cancellationToken);

            if (sensor is null)
            {
                return null;
            }

            sensor.Name = request.Name;
            sensor.SensorType = Enum.Parse<SAS.Backend.Domain.Enums.SensorType>(request.SensorType.ToString());
            sensor.MinValue = request.MinValue;
            sensor.MaxValue = request.MaxValue;
            sensor.Status = request.Status;
            sensor.InstalledAt = request.InstalledAt;

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<SensorDto>(sensor);
        }
    }
}

