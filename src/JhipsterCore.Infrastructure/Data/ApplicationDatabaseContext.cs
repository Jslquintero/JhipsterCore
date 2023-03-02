using JhipsterCore.Domain.Entities;
using JhipsterCore.Domain.Entities.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JhipsterCore.Infrastructure.Data;

public class ApplicationDatabaseContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    /// <summary>
    /// SaveChangesAsync with entities audit
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        var entries = ChangeTracker
          .Entries()
          .Where(e => e.Entity is IAuditedEntityBase && (
              e.State == EntityState.Added
              || e.State == EntityState.Modified));

        string modifiedOrCreatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                ((IAuditedEntityBase)entityEntry.Entity).CreatedDate = DateTime.Now;
                ((IAuditedEntityBase)entityEntry.Entity).CreatedBy = modifiedOrCreatedBy;
            }
            else
            {
                Entry((IAuditedEntityBase)entityEntry.Entity).Property(p => p.CreatedDate).IsModified = false;
                Entry((IAuditedEntityBase)entityEntry.Entity).Property(p => p.CreatedBy).IsModified = false;
            }
          ((IAuditedEntityBase)entityEntry.Entity).LastModifiedDate = DateTime.Now;
            ((IAuditedEntityBase)entityEntry.Entity).LastModifiedBy = modifiedOrCreatedBy;
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
