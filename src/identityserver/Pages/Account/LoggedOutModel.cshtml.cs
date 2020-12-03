// <copyright file="LoggedOutModel.cshtml.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Pages.Account
{
    using KclTracker.IdentityServer.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    [AllowAnonymous]
    public class LoggedOutModel : PageModel
    {
        public string PostLogoutRedirectUri { get; set; }

        public bool AutomaticRedirectAfterSignOut { get; set; }

        public void OnGet(string postLogoutRedirectUri)
        {
            this.PostLogoutRedirectUri = postLogoutRedirectUri;
            this.AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut;
        }
    }
}
