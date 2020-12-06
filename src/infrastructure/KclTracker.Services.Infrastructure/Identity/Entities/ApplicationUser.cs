// <copyright file="ApplicationUser.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Identity.Entities
{
    using KclTracker.Services.Application.Common;
    using KclTracker.Services.Domain.Entities;
    using KclTracker.Services.Domain.Enums;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser, IMapFrom<User>
    {
        public UserType UserTypeId { get; set; }

        public bool IsActive { get; set; }

        public ApplicationUserProfile Profile { get; set; }

        public ApplicationUserCompany Company { get; set; }
    }
}
