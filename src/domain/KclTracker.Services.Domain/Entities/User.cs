// <copyright file="User.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Domain.Entities
{
    using KclTracker.Services.Domain.Enums;

    public class User
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public UserType UserTypeId { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public UserProfile Profile { get; set; }

        public UserCompany Company { get; set; }
    }
}
