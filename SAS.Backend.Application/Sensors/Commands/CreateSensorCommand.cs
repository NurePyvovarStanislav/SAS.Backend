using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Sensors;
using SAS.Backend.Domain.Entities;

namespace SAS.Backend.Application.Sensors.Commands
{
    public record CreateSensorCommand(Guid FieldId, string Name, SAS.Backend.Contracts.Enums.SensorType SensorType, double MinValue, double MaxValue, string Status, DateTime InstalledAt) : IRequest<SensorDto?>;

    public class CreateSensorCommandHandler : IRequestHandler<CreateSensorCommand, SensorDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreateSensorCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SensorDto?> Handle(CreateSensorCommand request, CancellationToken cancellationToken)
        {
            var fieldExists = await _context.Fields
                .AsNoTracking()
                .AnyAsync(f => f.FieldId == request.FieldId, cancellationToken);

            if (!fieldExists)
            {
                return null;
            }

            var sensor = new Sensor
            {
                SensorId = Guid.NewGuid(),
                FieldId = request.FieldId,
                Name = request.Name,
                SensorType = Enum.Parse<SAS.Backend.Domain.Enums.SensorType>(request.SensorType.ToString()),
                MinValue = request.MinValue,
                MaxValue = request.MaxValue,
                Status = request.Status,
                InstalledAt = request.InstalledAt
            };

            _context.Sensors.Add(sensor);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<SensorDto>(sensor);
        }
    }
}

