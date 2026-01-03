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
    public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto?>;

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly JwtOptions _jwtOptions;

        public RefreshTokenCommandHandler(IApplicationDbContext context, ITokenService tokenService, IMapper mapper, IOptions<JwtOptions> jwtOptions)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<AuthResponseDto?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return null;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken);
            if (user is null)
            {
                return null;
            }

            if (!user.IsActive)
            {
                return null;
            }

            if (user.RefreshTokenExpiresAt is null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
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

