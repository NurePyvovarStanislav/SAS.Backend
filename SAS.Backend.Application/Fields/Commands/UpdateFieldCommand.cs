using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SAS.Backend.Application.Common.Interfaces;
using SAS.Backend.Contracts.Fields;

namespace SAS.Backend.Application.Fields.Commands
{
    public record UpdateFieldCommand(Guid FieldId, string Name, string CropType, double Area, string? Location) : IRequest<FieldDto?>;

    public class UpdateFieldCommandHandler : IRequestHandler<UpdateFieldCommand, FieldDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdateFieldCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<FieldDto?> Handle(UpdateFieldCommand request, CancellationToken cancellationToken)
        {
            var field = await _context.Fields
                .FirstOrDefaultAsync(f => f.FieldId == request.FieldId, cancellationToken);

            if (field is null)
            {
                return null;
            }

            field.Name = request.Name;
            field.CropType = request.CropType;
            field.Area = request.Area;
            field.Location = request.Location;

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<FieldDto>(field);
        }
    }
}

