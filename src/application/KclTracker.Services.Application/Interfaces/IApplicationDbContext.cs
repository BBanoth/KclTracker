// <copyright file="IApplicationDbContext.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Interfaces
{
    using KclTracker.Services.Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    public interface IApplicationDbContext
    {
        public DbSet<Company> Companies { get; set; }
    }
}
