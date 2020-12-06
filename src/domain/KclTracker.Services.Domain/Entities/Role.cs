// <copyright file="Role.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Domain.Entities
{
    using System.Collections.Generic;

    public class Role
    {
        public Role()
        {
            this.Claims = new List<System.Security.Claims.Claim>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public IList<System.Security.Claims.Claim> Claims { get; set; }
    }
}
