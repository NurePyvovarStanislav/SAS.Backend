using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Fields;

namespace SAS.Backend.Application.Fields.Queries
{
    public record GetFieldByIdQuery(Guid FieldId) : IRequest<FieldDto?>;

    public class GetFieldByIdQueryHandler : IRequestHandler<GetFieldByIdQuery, FieldDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetFieldByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<FieldDto?> Handle(GetFieldByIdQuery request, CancellationToken cancellationToken)
        {
            var field = await _context.Fields
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FieldId == request.FieldId, cancellationToken);

            return field is null ? null : _mapper.Map<FieldDto>(field);
        }
    }
}

