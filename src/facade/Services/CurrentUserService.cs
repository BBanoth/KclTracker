// <copyright file="CurrentUserService.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Facade
{
    using System.Collections.Generic;
    using System.Linq;
    using KclTracker.Services.Application.Interfaces;
    using KclTracker.Services.Facade.Extensions;
    using Microsoft.AspNetCore.Http;

    public class CurrentUserService : ICurrentUserService
    {
        private readonly HttpContext _context;

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            this._context = contextAccessor?.HttpContext;
        }

        public string UserId => this._context?.User?.GetSubjectId();

        public string SessionId => this._context?.User?.GetSessionId();

        public bool IsAuthenticated => this._context?.User?.Identity?.IsAuthenticated == true;

        public IList<ClaimModel> Claims => this._context?.User?.Claims?.Select(claim => new ClaimModel(claim.Type, claim.Value))?.ToList() ?? new List<ClaimModel>();

        public bool IsInClaim(string claim)
        {
            return this.Claims.Any(x => x.Type == claim && x.Value == "true");
        }
    }
}
