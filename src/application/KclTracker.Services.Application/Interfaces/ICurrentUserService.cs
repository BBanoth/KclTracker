// <copyright file="ICurrentUserService.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Interfaces
{
    using System.Collections.Generic;

    // Should be accessed after authentication and authorization pipelines
    public interface ICurrentUserService
    {
        string UserId { get; }

        string SessionId { get; }

        bool IsAuthenticated { get; }

        IList<ClaimModel> Claims { get; }

        bool IsInClaim(string claim);
    }
}
