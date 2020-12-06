// <copyright file="ApplicationUserProfileConfiguration.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Identity.Configurations
{
    using KclTracker.Services.Infrastructure.Identity.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ApplicationUserProfileConfiguration : IEntityTypeConfiguration<ApplicationUserProfile>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserProfile> builder)
        {
            builder.ToTable("UserProfile")
                .HasKey(profile => profile.Id);

            builder.Property(profile => profile.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
