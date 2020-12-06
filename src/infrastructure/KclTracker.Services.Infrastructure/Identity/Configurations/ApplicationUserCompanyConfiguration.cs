// <copyright file="ApplicationUserCompanyConfiguration.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Identity.Configurations
{
    using KclTracker.Services.Infrastructure.Identity.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ApplicationUserCompanyConfiguration : IEntityTypeConfiguration<ApplicationUserCompany>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserCompany> builder)
        {
            builder.ToTable("UserCompany")
                .HasKey(profile => profile.Id);

            builder.Property(company => company.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
