// <copyright file="ApplicationUserCompany.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Identity.Entities
{
    using KclTracker.Services.Application.Common;
    using KclTracker.Services.Domain.Entities;

    public class ApplicationUserCompany : IMapFrom<UserCompany>
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int CompanyId { get; set; }
    }
}
