// <copyright file="LoginModel.cshtml.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Pages.Account
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using IdentityServer4.Events;
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
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IEventService _eventService;
        private readonly IIdentityServerInteractionService _interaction;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger,
            IEventService eventService,
            IIdentityServerInteractionService interaction)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._logger = logger;
            this._eventService = eventService;
            this._interaction = interaction;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(this.ErrorMessage))
            {
                this.ModelState.AddModelError(string.Empty, this.ErrorMessage);
            }

            // Clear the existing external cookie to ensure a clean login process
            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            this.ExternalLogins = (await this._signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            returnUrl ??= this.Url.Content("~/");

            this.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= this.Url.Content("~/");

            this.ExternalLogins = (await this._signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            this.ReturnUrl = returnUrl;

            if (this.ModelState.IsValid)
            {
                var user = await this._userManager.FindByEmailAsync(this.Input.Email);

                if (user == null)
                {
                    this._logger.LogWarning($"User does not exists with the provided email: {this.Input.Email}");
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return this.Page();
                }

                if (!user.IsActive)
                {
                    this._logger.LogWarning($"User is currently not active: {this.Input.Email}");
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return this.Page();
                }

                // check if we are in the context of an authorization request
                var context = await this._interaction.GetAuthorizationContextAsync(returnUrl);

                var result = await this._signInManager.PasswordSignInAsync(user, this.Input.Password, isPersistent: false, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    this._logger.LogInformation($"User logged in: {this.Input.Email}");

                    await this._eventService.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

                    if (context != null)
                    {
                        if (context.IsNativeClient())
                        {
                            // if the client is PKCE then we assume it's native, so this change in how to
                            // return the response is for better UX for the end user.
                            return this.RedirectToPage("/Redirect", new { RedirectUrl = returnUrl });
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return this.Redirect(returnUrl);
                    }

                    // request for a local page
                    if (this.Url.IsLocalUrl(returnUrl))
                    {
                        return this.Redirect(returnUrl);
                    }
                    else if (string.IsNullOrEmpty(returnUrl))
                    {
                        return this.Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new InvalidOperationException("Invalid redirect url");
                    }
                }

                await this._eventService.RaiseAsync(new UserLoginFailureEvent(this.Input.Email, "Login failed", clientId: context?.Client.ClientId));

                if (result.IsLockedOut)
                {
                    this._logger.LogWarning($"User account locked out: {this.Input.Email}");
                    return this.RedirectToPage("./Lockout");
                }

                this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return this.Page();
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }
    }
}
