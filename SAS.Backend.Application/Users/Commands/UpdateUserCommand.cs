using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Users;

namespace SAS.Backend.Application.Users.Commands
{
    public record UpdateUserCommand(Guid UserId, string Email, string FullName, SAS.Backend.Contracts.Enums.UserRole Role, string? Phone, string? Password, Guid? FieldId) : IRequest<UserDto?>;

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);
            if (user is null) return null;

            var emailExists = await _context.Users.AnyAsync(u => u.Email == request.Email && u.UserId != request.UserId, cancellationToken);
            if (emailExists) return null;

            user.Email = request.Email;
            user.FullName = request.FullName;
            user.Role = Enum.Parse<SAS.Backend.Domain.Enums.UserRole>(request.Role.ToString());
            user.Phone = request.Phone;
            user.FieldId = request.FieldId;
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return _mapper.Map<UserDto>(user);
        }
    }
}

