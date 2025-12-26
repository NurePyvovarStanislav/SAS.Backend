using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Fields;

namespace SAS.Backend.Application.Fields.Queries
{
    public record GetFieldsQuery : IRequest<IReadOnlyCollection<FieldDto>>;

    public class GetFieldsQueryHandler : IRequestHandler<GetFieldsQuery, IReadOnlyCollection<FieldDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetFieldsQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<FieldDto>> Handle(GetFieldsQuery request, CancellationToken cancellationToken)
        {
            var fields = await _context.Fields
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyCollection<FieldDto>>(fields);
        }
    }
}

