// <copyright file="UserProfileConfiguration.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Data.Configurations
{
    using KclTracker.IdentityServer.Data.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable(nameof(UserProfile))
                .HasKey(profile => profile.Id);
        }
    }
}
