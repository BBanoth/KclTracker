// <copyright file="CompanyConfiguration.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Persistence.Configurations
{
    using KclTracker.Services.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.ToTable(nameof(Shipment));

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.InspectionType)
                .WithOne()
                .HasForeignKey<Shipment>(x => x.InspectionTypeId);

            builder.HasMany(x => x.History)
                .WithOne()
                .HasForeignKey(x => x.ShipmentId);
        }
    }
}
