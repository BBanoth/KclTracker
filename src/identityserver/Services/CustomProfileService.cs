// <copyright file="CustomProfileService.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Services
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using IdentityModel;
    using IdentityServer4.AspNetIdentity;
    using IdentityServer4.Extensions;
    using IdentityServer4.Models;
    using KclTracker.IdentityServer.Data;
    using KclTracker.IdentityServer.Data.Entities;
    using KclTracker.IdentityServer.Extensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class CustomProfileService : ProfileService<ApplicationUser>
    {
        private readonly UserIdentityDbContext _context;

        public CustomProfileService(
            [NotNull] UserManager<ApplicationUser> userManager,
            [NotNull] IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            [NotNull] UserIdentityDbContext context)
            : base(userManager, claimsFactory)
        {
            this._context = context;
        }

        public CustomProfileService(
            [NotNull] UserManager<ApplicationUser> userManager,
            [NotNull] IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            [NotNull] ILogger<ProfileService<ApplicationUser>> logger,
            [NotNull] UserIdentityDbContext context)
            : base(userManager, claimsFactory, logger)
        {
            this._context = context;
        }

        public override async Task IsActiveAsync(IsActiveContext context)
        {
            await base.IsActiveAsync(context);

            var user = await this.UserManager.FindByIdAsync(context.Subject?.GetSubjectId());

            context.IsActive = user != null && user.IsActive == true;
        }

        protected override async Task<ClaimsPrincipal> GetUserClaimsAsync(ApplicationUser user)
        {
            var profile = await this._context.UserProfiles.FirstOrDefaultAsync(profile => profile.UserId == user.Id);

            if (profile != null)
            {
                var principal = await base.GetUserClaimsAsync(user);

                var identity = principal.Identities.First();

                if (!identity.HasClaim(x => x.Type == JwtClaimTypes.GivenName) && profile.FirstName.IsPresent())
                {
                    identity.AddClaim(new Claim(JwtClaimTypes.GivenName, profile.FirstName));
                }

                if (!identity.HasClaim(x => x.Type == JwtClaimTypes.FamilyName) && profile.LastName.IsPresent())
                {
                    identity.AddClaim(new Claim(JwtClaimTypes.FamilyName, profile.LastName));
                }

                return principal;
            }

            return await base.GetUserClaimsAsync(user);
        }
    }
}
