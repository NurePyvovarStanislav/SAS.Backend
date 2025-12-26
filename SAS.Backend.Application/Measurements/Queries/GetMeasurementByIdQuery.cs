using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Measurements;

namespace SAS.Backend.Application.Measurements.Queries
{
    public record GetMeasurementByIdQuery(Guid MeasurementId) : IRequest<MeasurementDto?>;

    public class GetMeasurementByIdQueryHandler : IRequestHandler<GetMeasurementByIdQuery, MeasurementDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetMeasurementByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MeasurementDto?> Handle(GetMeasurementByIdQuery request, CancellationToken cancellationToken)
        {
            var measurement = await _context.Measurements.AsNoTracking()
                .FirstOrDefaultAsync(m => m.MeasurementId == request.MeasurementId, cancellationToken);
            return measurement is null ? null : _mapper.Map<MeasurementDto>(measurement);
        }
    }
}

