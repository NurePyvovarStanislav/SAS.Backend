using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Users;
using SAS.Backend.Domain.Entities;

namespace SAS.Backend.Application.Users.Commands
{
    public record CreateUserCommand(string Email, string Password, string FullName, SAS.Backend.Contracts.Enums.UserRole Role, string? Phone, Guid? FieldId) : IRequest<UserDto?>;

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto?> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
            if (exists) return null;

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName,
                Role = Enum.Parse<SAS.Backend.Domain.Enums.UserRole>(request.Role.ToString()),
                Phone = request.Phone,
                FieldId = request.FieldId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<UserDto>(user);
        }
    }
}

