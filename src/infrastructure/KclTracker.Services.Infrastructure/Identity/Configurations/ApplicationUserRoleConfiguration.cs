// <copyright file="ApplicationUserRoleConfiguration.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Identity.Configurations
{
    using KclTracker.Services.Infrastructure.Identity.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
        {
            builder.HasOne(userRole => userRole.Role)
                .WithOne()
                .HasForeignKey<ApplicationUserRole>(userRole => userRole.RoleId);
        }
    }
}
