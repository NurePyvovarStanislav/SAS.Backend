using AutoMapper;
using MediatR;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Fields;
using SAS.Backend.Domain.Entities;

namespace SAS.Backend.Application.Fields.Commands
{
    public record CreateFieldCommand(string Name, string CropType, double Area, string? Location, Guid UserId) : IRequest<FieldDto>;

    public class CreateFieldCommandHandler : IRequestHandler<CreateFieldCommand, FieldDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreateFieldCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<FieldDto> Handle(CreateFieldCommand request, CancellationToken cancellationToken)
        {
            var field = new Field
            {
                FieldId = Guid.NewGuid(),
                Name = request.Name,
                CropType = request.CropType,
                Area = request.Area,
                Location = request.Location,
                UserId = request.UserId
            };

            _context.Fields.Add(field);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<FieldDto>(field);
        }
    }
}

