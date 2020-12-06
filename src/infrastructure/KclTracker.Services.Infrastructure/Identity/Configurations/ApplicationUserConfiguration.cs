// <copyright file="ApplicationUserConfiguration.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Identity.Configurations
{
    using KclTracker.Services.Infrastructure.Identity.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(user => user.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(user => user.Profile)
                .WithOne()
                .HasForeignKey<ApplicationUserProfile>(profile => profile.UserId);

            builder.HasOne(user => user.Company)
                .WithOne()
                .HasForeignKey<ApplicationUserCompany>(profile => profile.UserId);
        }
    }
}
