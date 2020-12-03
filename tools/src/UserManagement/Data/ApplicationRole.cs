// <copyright file="ApplicationRole.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Tools.UserManagement.Data
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationRole : IdentityRole<string>
    {
        public ApplicationRole()
        {
            this.Claims = new List<IdentityRoleClaim<string>>();
        }

        public ICollection<IdentityRoleClaim<string>> Claims { get; private set; }
    }
}
