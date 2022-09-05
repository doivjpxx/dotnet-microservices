using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Commons;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence;

public class OrderContext : DbContext
{
    public OrderContext(DbContextOptions<OrderContext> options) : base(options)
    {
        
    }
    
    public DbSet<Order> Orders { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = DateTime.Now;
                    entry.Entity.CreatedBy = "phong";
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = "phong";
                    entry.Entity.LastModifiedDate = DateTime.Now;
                    break;
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}