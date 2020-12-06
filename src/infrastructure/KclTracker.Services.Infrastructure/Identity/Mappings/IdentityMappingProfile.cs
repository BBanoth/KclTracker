// <copyright file="IdentityMappingProfile.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Identity.Mappings
{
    using System.Reflection;
    using KclTracker.Services.Application.Common;

    public class IdentityMappingProfile : MappingProfile
    {
        public IdentityMappingProfile()
            : this(Assembly.GetExecutingAssembly())
        {
        }

        public IdentityMappingProfile(Assembly assembly)
            : base(assembly)
        {
        }
    }
}
