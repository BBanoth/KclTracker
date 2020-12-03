// <copyright file="RedirectModel.cshtml.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Pages.Shared
{
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class RedirectModel : PageModel
    {
        public string RedirectUrl { get; set; }

        public void OnGet(string redirectUrl = null)
        {
            this.RedirectUrl = redirectUrl;
        }
    }
}
