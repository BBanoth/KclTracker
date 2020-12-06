// <copyright file="UserCompany.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Domain.Entities
{
    public class UserCompany
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int CompanyId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}
