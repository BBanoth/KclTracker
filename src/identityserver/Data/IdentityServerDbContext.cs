// <copyright file="IdentityServerDbContext.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Data
{
    using System.Threading.Tasks;
    using IdentityServer4.EntityFramework.Entities;
    using IdentityServer4.EntityFramework.Extensions;
    using IdentityServer4.EntityFramework.Interfaces;
    using IdentityServer4.EntityFramework.Options;
    using KclTracker.IdentityServer.Extensions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class IdentityServerDbContext : DbContext, IPersistedGrantDbContext
    {
        private readonly OperationalStoreOptions _storeOptions;
        private readonly IConfiguration _configuration;

        public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options, OperationalStoreOptions storeOptions, IConfiguration configuration)
            : base(options)
        {
            this._storeOptions = storeOptions;
            this._configuration = configuration;
        }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

        public Task<int> SaveChangesAsync() => base.SaveChangesAsync();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this._configuration.GetConnectionString());

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ConfigurePersistedGrantContext(this._storeOptions);

            base.OnModelCreating(builder);
        }
    }
}
