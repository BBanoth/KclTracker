// <copyright file="CompanyConfiguration.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Persistence.Configurations
{
    using KclTracker.Services.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ShipmentHistoryConfiguration : IEntityTypeConfiguration<ShipmentHistory>
    {
        public void Configure(EntityTypeBuilder<ShipmentHistory> builder)
        {
            builder.ToTable(nameof(ShipmentHistory));

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Status)
                .WithOne()
                .HasForeignKey<ShipmentHistory>(x => x.ShipmentStatusId);
        }
    }
}
