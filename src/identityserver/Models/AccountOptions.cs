// <copyright file="AccountOptions.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Models
{
    using System;
    using Microsoft.AspNetCore.Server.IISIntegration;

    public class AccountOptions
    {
        // specify the Windows authentication scheme being used
        public static readonly string WindowsAuthenticationSchemeName = IISDefaults.AuthenticationScheme;

        public static TimeSpan RememberMeLoginDuration { get; set; } = TimeSpan.FromDays(30);

        public static bool ShowLogoutPrompt { get; set; } = true;

        public static bool AutomaticRedirectAfterSignOut { get; set; } = false;
    }
}
