using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Measurements;

namespace SAS.Backend.Application.Measurements.Queries
{
    public record GetMeasurementsBySensorQuery(Guid SensorId, DateTime? From, DateTime? To) : IRequest<IReadOnlyCollection<MeasurementDto>>;

    public class GetMeasurementsBySensorQueryHandler : IRequestHandler<GetMeasurementsBySensorQuery, IReadOnlyCollection<MeasurementDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetMeasurementsBySensorQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<MeasurementDto>> Handle(GetMeasurementsBySensorQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Measurements.AsNoTracking().Where(m => m.SensorId == request.SensorId);
            if (request.From.HasValue)
            {
                query = query.Where(m => m.MeasuredAt >= request.From.Value);
            }
            if (request.To.HasValue)
            {
                query = query.Where(m => m.MeasuredAt <= request.To.Value);
            }

            var list = await query.OrderByDescending(m => m.MeasuredAt).ToListAsync(cancellationToken);
            return _mapper.Map<IReadOnlyCollection<MeasurementDto>>(list);
        }
    }
}

