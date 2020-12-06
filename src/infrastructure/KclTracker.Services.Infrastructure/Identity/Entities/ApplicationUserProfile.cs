// <copyright file="ApplicationUserProfile.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Identity.Entities
{
    using AutoMapper;
    using KclTracker.Services.Application.Common;
    using KclTracker.Services.Domain.Entities;

    public class ApplicationUserProfile : IMapFrom<UserProfile>
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ApplicationUserProfile, UserProfile>()
                .ReverseMap();
        }
    }
}
