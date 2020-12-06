// <copyright file="ApplicationRole.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Identity.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using KclTracker.Services.Application.Common;
    using KclTracker.Services.Domain.Entities;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationRole : IdentityRole, IMapFrom<Role>
    {
        public ApplicationRole()
        {
            this.Claims = new HashSet<IdentityRoleClaim<string>>();
        }

        public ICollection<IdentityRoleClaim<string>> Claims { get; private set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ApplicationRole, Role>()
                .ForMember(d => d.Claims, opt => opt.MapFrom(s => s.Claims.Select(claim => new System.Security.Claims.Claim(claim.ClaimType, claim.ClaimValue))));

            profile.CreateMap<Role, ApplicationRole>()
                .ForMember(d => d.Claims, opt => opt.Ignore());
        }
    }
}
