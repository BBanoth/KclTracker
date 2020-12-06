// <copyright file="BaseEntity.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Domain.Entities
{
    using System;

    public abstract class BaseEntity : BaseAuditEntity
    {
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public virtual void ApplyDelete()
        {
            this.IsActive = false;
            this.IsDeleted = true;
        }
    }
}
