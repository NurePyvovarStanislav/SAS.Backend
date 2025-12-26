using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Sensors;

namespace SAS.Backend.Application.Sensors.Queries
{
    public record GetSensorsByFieldQuery(Guid FieldId) : IRequest<IReadOnlyCollection<SensorDto>>;

    public class GetSensorsByFieldQueryHandler : IRequestHandler<GetSensorsByFieldQuery, IReadOnlyCollection<SensorDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetSensorsByFieldQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<SensorDto>> Handle(GetSensorsByFieldQuery request, CancellationToken cancellationToken)
        {
            var sensors = await _context.Sensors
                .AsNoTracking()
                .Where(s => s.FieldId == request.FieldId)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyCollection<SensorDto>>(sensors);
        }
    }
}

