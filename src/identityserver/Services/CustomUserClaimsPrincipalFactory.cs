// <copyright file="CustomUserClaimsPrincipalFactory.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Services
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;

    internal class CustomUserClaimsPrincipalFactory<TUser> : UserClaimsPrincipalFactory<TUser>
        where TUser : class
    {
        public CustomUserClaimsPrincipalFactory(UserManager<TUser> userManager, IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
        {
            var userId = await this.UserManager.GetUserIdAsync(user);
            var userName = await this.UserManager.GetUserNameAsync(user);

            var id = new ClaimsIdentity("Identity.Application", this.Options.ClaimsIdentity.UserNameClaimType, this.Options.ClaimsIdentity.RoleClaimType);
            id.AddClaim(new Claim(this.Options.ClaimsIdentity.UserIdClaimType, userId));
            id.AddClaim(new Claim(this.Options.ClaimsIdentity.UserNameClaimType, userName));

            if (this.UserManager.SupportsUserSecurityStamp)
            {
                id.AddClaim(new Claim(this.Options.ClaimsIdentity.SecurityStampClaimType, await this.UserManager.GetSecurityStampAsync(user)));
            }

            return id;
        }
    }
}
