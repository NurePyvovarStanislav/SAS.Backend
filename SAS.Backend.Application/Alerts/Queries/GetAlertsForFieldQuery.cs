using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Alerts;

namespace SAS.Backend.Application.Alerts.Queries
{
    public record GetAlertsForFieldQuery(Guid FieldId) : IRequest<IReadOnlyCollection<AlertDto>>;

    public class GetAlertsForFieldQueryHandler : IRequestHandler<GetAlertsForFieldQuery, IReadOnlyCollection<AlertDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAlertsForFieldQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<AlertDto>> Handle(GetAlertsForFieldQuery request, CancellationToken cancellationToken)
        {
            var alerts = await _context.Alerts
                .AsNoTracking()
                .Include(a => a.Measurement)
                .ThenInclude(m => m.Sensor)
                .Where(a => a.Measurement.Sensor.FieldId == request.FieldId)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyCollection<AlertDto>>(alerts);
        }
    }
}

