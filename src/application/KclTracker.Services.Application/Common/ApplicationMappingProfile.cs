// <copyright file="ApplicationMappingProfile.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Common
{
    using System.Reflection;

    public class ApplicationMappingProfile : MappingProfile
    {
        public ApplicationMappingProfile()
            : this(Assembly.GetExecutingAssembly())
        {
        }

        public ApplicationMappingProfile(Assembly assembly)
            : base(assembly)
        {
        }
    }
}
