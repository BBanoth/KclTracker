// <copyright file="LogoutModel.cshtml.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Pages.Account
{
    using System.Threading.Tasks;
    using IdentityModel;
    using IdentityServer4;
    using IdentityServer4.Events;
    using IdentityServer4.Extensions;
    using IdentityServer4.Services;
    using KclTracker.IdentityServer.Data.Entities;
    using KclTracker.IdentityServer.Extensions;
    using KclTracker.IdentityServer.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;

    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly IUserSession _userSession;

        public LogoutModel(
            SignInManager<ApplicationUser> signInManager,
            ILogger<LogoutModel> logger,
            IIdentityServerInteractionService interaction,
            IEventService events,
            IUserSession userSession)
        {
            this._signInManager = signInManager;
            this._logger = logger;
            this._interaction = interaction;
            this._events = events;
            this._userSession = userSession;
        }

        public bool ShowLogoutPrompt { get; private set; }

        public string LogoutId { get; set; }

        public async Task<IActionResult> OnGetAsync(string logoutId)
        {
            // build a model so the logout page knows what to display
            await this.BuildLogoutViewModelAsync(logoutId);

            if (!this.ShowLogoutPrompt)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await this.OnPostAsync(logoutId);
            }

            this.LogoutId = logoutId;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string logoutId = null)
        {
            // build a model so the logged out page knows what to display
            var vm = await this.BuildLoggedOutViewModelAsync(logoutId);

            if (this.User?.Identity.IsAuthenticated == true)
            {
                // Remove user cache on signout
                var sub = this.User.FindFirst(JwtClaimTypes.Subject)?.Value;

                var sessionId = await this._userSession.GetSessionIdAsync();

                // delete local authentication cookie
                await this._signInManager.SignOutAsync();

                // raise the logout event
                await this._events.RaiseAsync(new UserLogoutSuccessEvent(sub, this.User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.ExternalAuthenticationScheme.IsPresent())
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = this.Url.Page("./Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return this.SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            if (!string.IsNullOrWhiteSpace(vm.PostLogoutRedirectUri))
            {
                return this.Redirect(vm.PostLogoutRedirectUri);
            }

            return this.RedirectToPage("./LoggedOut", new { vm.PostLogoutRedirectUri });
        }

        public async Task<IActionResult> OnPostCancelAsync(string logoutId = null)
        {
            var logout = await this._interaction.GetLogoutContextAsync(logoutId);

            var returnUrl = logout?.PostLogoutRedirectUri ?? this.Url.Content("~/");

            return this.Redirect(returnUrl);
        }

        private async Task BuildLogoutViewModelAsync(string logoutId)
        {
            this.ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt;

            if (this.User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                this.ShowLogoutPrompt = false;
                return;
            }

            var context = await this._interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                this.ShowLogoutPrompt = false;
            }
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await this._interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri
            };

            if (this.User?.Identity.IsAuthenticated == true)
            {
                var idp = this.User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await this.HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (logoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await this._interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }
    }
}
