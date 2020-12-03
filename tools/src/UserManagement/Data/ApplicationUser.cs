// <copyright file="ApplicationUser.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Tools.UserManagement.Data
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.UserLogins = new HashSet<IdentityUserLogin<string>>();
        }

        public bool IsActive { get; set; }

        public UserProfile Profile { get; set; }

        public UserType UserTypeId { get; set; }

        public ICollection<IdentityUserLogin<string>> UserLogins { get; private set; }
    }
}
