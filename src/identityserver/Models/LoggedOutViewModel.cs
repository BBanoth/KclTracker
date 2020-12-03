// <copyright file="LoggedOutViewModel.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Models
{
    public class LoggedOutViewModel
    {
        public string PostLogoutRedirectUri { get; set; }

        public bool AutomaticRedirectAfterSignOut { get; set; }

        public string ExternalAuthenticationScheme { get; set; }

        public string LogoutId { get; set; }
    }
}
