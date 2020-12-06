// <copyright file="BaseAuditEntity.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Domain.Entities
{
    using System;

    public abstract class BaseAuditEntity
    {
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public void SetAuditData(string createdBy, DateTime createdOn)
        {
            this.CreatedBy = createdBy;
            this.CreatedOn = createdOn;
        }
    }
}
