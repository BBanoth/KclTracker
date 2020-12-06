// <copyright file="ClaimsPrincipalExtensions.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Facade.Extensions
{
    using System.Security.Claims;
    using IdentityModel;

    public static class ClaimsPrincipalExtensions
    {
        public static string GetSubjectId(this ClaimsPrincipal principal)
        {
            return GetClaim(principal, JwtClaimTypes.Subject)?.Value;
        }

        public static string GetSessionId(this ClaimsPrincipal principal)
        {
            return GetClaim(principal, JwtClaimTypes.SessionId)?.Value;
        }

        private static Claim GetClaim(ClaimsPrincipal principal, string claimType)
        {
            var identity = principal?.Identity as ClaimsIdentity;
            return identity?.FindFirst(claimType);
        }
    }
}
