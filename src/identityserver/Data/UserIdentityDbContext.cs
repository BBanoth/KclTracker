// <copyright file="UserIdentityDbContext.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Data
{
    using KclTracker.IdentityServer.Data.Entities;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class UserIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public UserIdentityDbContext(DbContextOptions<UserIdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("security");

            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(UserIdentityDbContext).Assembly);
        }
    }
}
