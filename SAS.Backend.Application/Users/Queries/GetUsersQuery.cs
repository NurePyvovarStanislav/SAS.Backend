using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Users;

namespace SAS.Backend.Application.Users.Queries
{
    public record GetUsersQuery : IRequest<IReadOnlyCollection<UserDto>>;

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IReadOnlyCollection<UserDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _context.Users.AsNoTracking().ToListAsync(cancellationToken);
            return _mapper.Map<IReadOnlyCollection<UserDto>>(users);
        }
    }
}

