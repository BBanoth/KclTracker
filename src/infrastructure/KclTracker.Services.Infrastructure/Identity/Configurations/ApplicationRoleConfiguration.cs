// <copyright file="ApplicationRoleConfiguration.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Identity.Configurations
{
    using KclTracker.Services.Infrastructure.Identity.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.Property(user => user.Id)
              .ValueGeneratedOnAdd();

            builder.HasMany(role => role.Claims)
                .WithOne()
                .HasForeignKey(roleClaim => roleClaim.RoleId);
        }
    }
}
