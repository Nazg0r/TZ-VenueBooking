using Microsoft.EntityFrameworkCore;

using VenueBooking.DataAccess.Data;
using VenueBooking.Domain.Interfaces.Repositories;
using VenueBooking.Domain.Models;

namespace VenueBooking.DataAccess.Repositories;

public class BaseRepository<T>(VenueBookingContext context) : IRepository<T>
    where T : Entity
{
    protected VenueBookingContext Context { get; } = context;

    protected DbSet<T> Set => Context.Set<T>();

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await Set.FindAsync([id], cancellationToken);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await Set.AsNoTracking().ToListAsync(cancellationToken);

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Set.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        Set.Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        Set.Remove(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }
}