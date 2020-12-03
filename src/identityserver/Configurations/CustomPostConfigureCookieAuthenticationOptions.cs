// <copyright file="CustomPostConfigureCookieAuthenticationOptions.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Configurations
{
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.Extensions.Options;

    public class CustomPostConfigureCookieAuthenticationOptions : IPostConfigureOptions<CookieAuthenticationOptions>
    {
        public void PostConfigure(string name, CookieAuthenticationOptions options)
        {
            options.CookieManager = new ChunkingCookieManager();
        }
    }
}
