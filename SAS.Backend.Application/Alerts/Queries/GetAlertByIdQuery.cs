using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Alerts;

namespace SAS.Backend.Application.Alerts.Queries
{
    public record GetAlertByIdQuery(Guid AlertId) : IRequest<AlertDto?>;

    public class GetAlertByIdQueryHandler : IRequestHandler<GetAlertByIdQuery, AlertDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAlertByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AlertDto?> Handle(GetAlertByIdQuery request, CancellationToken cancellationToken)
        {
            var alert = await _context.Alerts.AsNoTracking()
                .FirstOrDefaultAsync(a => a.AlertId == request.AlertId, cancellationToken);

            return alert is null ? null : _mapper.Map<AlertDto>(alert);
        }
    }
}

