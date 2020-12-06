// <copyright file="ApplicationUserRole.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Identity.Entities
{
    using AutoMapper;
    using KclTracker.Services.Application.Common;
    using KclTracker.Services.Domain.Entities;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUserRole : IdentityUserRole<string>, IMapFrom<UserRole>
    {
        public ApplicationRole Role { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ApplicationUserRole, UserRole>()
                .ReverseMap();
        }
    }
}
