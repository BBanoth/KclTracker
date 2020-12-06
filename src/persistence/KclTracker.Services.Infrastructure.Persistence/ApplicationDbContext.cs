// <copyright file="ApplicationDbContext.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Persistence
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KclTracker.Services.Application.Interfaces;
    using KclTracker.Services.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUser;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUser)
            : base(options)
        {
            this._currentUser = currentUser;
        }

        public DbSet<Company> Companies { get; set; }

        public DbSet<UserCompany> UserCompanies { get; set; }

        public DbSet<Shipment> Shipments { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override EntityEntry Update(object entity)
        {
            return base.Update(entity);
        }

        public override EntityEntry Entry(object entity)
        {
            return base.Entry(entity);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        private void OnBeforeSaving()
        {
            var entries = this.ChangeTracker.Entries()
                .Where(entry => (entry.Entity is BaseEntity || entry.Entity is BaseAuditEntity) && (entry.State == EntityState.Added || entry.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.Entity is BaseEntity entity)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            entity.ModifiedOn = now;
                            entity.ModifiedBy = this._currentUser.UserId;
                            entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                            entry.Property(nameof(BaseEntity.CreatedOn)).IsModified = false;
                            break;

                        case EntityState.Added:
                            entity.CreatedOn = now;
                            entity.CreatedBy = this._currentUser.UserId;
                            break;
                    }
                }
                else if (entry.Entity is BaseAuditEntity auditEntity)
                {
                    auditEntity.CreatedOn = DateTime.UtcNow;
                    auditEntity.CreatedBy = this._currentUser.UserId;
                }
            }
        }
    }
}
