// <copyright file="CompanyListVm.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Companies.Queries
{
    using KclTracker.Services.Application.Common;
    using KclTracker.Services.Domain.Entities;

    public class CompanyListVm : IMapFrom<Company>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
