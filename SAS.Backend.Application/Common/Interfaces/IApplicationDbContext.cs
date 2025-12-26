using Microsoft.EntityFrameworkCore;
using SAS.Backend.Domain.Entities;

namespace SAS.Backend.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Field> Fields { get; }
        DbSet<Sensor> Sensors { get; }
        DbSet<Measurement> Measurements { get; }
        DbSet<Alert> Alerts { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

