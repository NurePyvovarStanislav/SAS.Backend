using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Application.Common.Settings;
using SAS.Backend.Contracts.Auth;
using SAS.Backend.Contracts.Users;

namespace SAS.Backend.Application.Auth.Commands
{
    public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto?>;

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly JwtOptions _jwtOptions;

        public LoginCommandHandler(IApplicationDbContext context, ITokenService tokenService, IMapper mapper, IOptions<JwtOptions> jwtOptions)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<AuthResponseDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (user is null)
            {
                return null;
            }

            if (!user.IsActive)
            {
                return null;
            }

            var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!passwordValid)
            {
                return null;
            }

            var tokens = _tokenService.GenerateTokenPair(user);
            user.RefreshToken = tokens.RefreshToken;
            user.RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt;

            await _context.SaveChangesAsync(cancellationToken);

            var userDto = _mapper.Map<UserDto>(user);
            return new AuthResponseDto(tokens.AccessToken, tokens.AccessTokenExpiresAt, tokens.RefreshToken, tokens.RefreshTokenExpiresAt, userDto);
        }
    }
}

