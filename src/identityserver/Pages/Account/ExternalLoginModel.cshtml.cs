// <copyright file="ExternalLoginModel.cshtml.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Pages.Account
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using IdentityModel;
    using IdentityServer4;
    using IdentityServer4.Events;
    using IdentityServer4.Services;
    using KclTracker.IdentityServer.Constants;
    using KclTracker.IdentityServer.Data.Entities;
    using KclTracker.IdentityServer.Extensions;
    using KclTracker.IdentityServer.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;

    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly IEventService _eventService;
        private readonly IIdentityServerInteractionService _interaction;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IEventService eventService,
            IIdentityServerInteractionService interaction)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._logger = logger;
            this._eventService = eventService;
            this._interaction = interaction;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            return this.RedirectToPage("./Login");
        }

        public async Task<IActionResult> OnPostAsync(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = this.Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = this._signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            if (provider == AccountOptions.WindowsAuthenticationSchemeName)
            {
                // windows authentication needs special handling
                return await this.ProcessWindowsLoginAsync(provider, properties, returnUrl);
            }

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= this.Url.Content("~/");

            if (remoteError != null)
            {
                this._logger.LogError($"External login error: {remoteError}");

                return this.LoginError(returnUrl, string.Format(LoginErrorConstants.ExternalRemoteError, remoteError));
            }

            // read external identity from the temporary cookie
            var result = await this.HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (result?.Succeeded != true)
            {
                this._logger.LogError($"External login error");

                return this.LoginError(returnUrl);
            }

            if (this._logger.IsEnabled(LogLevel.Debug))
            {
                this._logger.LogDebug("External claims: {@claims}", result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}"));
            }

            var info = await this._signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                this._logger.LogError($"Configured external login information before the authentication is missing");

                return this.LoginError(returnUrl);
            }

            var userId = info.Principal.Identity.Name;

            var user = await this._userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    this._logger.LogError($"External login with provider {info.LoginProvider} is success but the user principal name is missing");

                    return this.LoginError(returnUrl);
                }

                // Check for email claim in external authentication principal
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    user = await this._userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
                }

                if (user == null || !user.IsActive)
                {
                    return this.UserNotRegistered(returnUrl, userId);
                }

                // Map user to user logins
                var identityResult = await this._userManager.AddLoginAsync(user, new UserLoginInfo(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName));
                if (!identityResult.Succeeded)
                {
                    this._logger.LogError($"External login with provider {info.LoginProvider} is success but mapping the user to userlogins is failed");

                    return this.LoginError(returnUrl);
                }
            }

            if (!user.IsActive)
            {
                return this.UserNotRegistered(returnUrl, userId);
            }

            // this allows us to collect any additonal claims or properties
            // for the specific prtotocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            this.ProcessLoginCallbackForOidc(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            // we must issue the cookie maually, and can't use the SignInManager because
            // it doesn't expose an API to issue additional claims from the login workflow
            var principal = await this._signInManager.CreateUserPrincipalAsync(user);
            additionalLocalClaims.AddRange(principal.Claims);

            var name = principal.FindFirst(JwtClaimTypes.Name)?.Value ?? user.Id;

            var isuser = new IdentityServerUser(user.Id)
            {
                DisplayName = name,
                IdentityProvider = info.LoginProvider,
                AdditionalClaims = additionalLocalClaims
            };

            await this.HttpContext.SignInAsync(isuser, localSignInProps);

            await this._eventService.RaiseAsync(new UserLoginSuccessEvent(info.LoginProvider, info.ProviderKey, user.Id, name));

            // delete temporary cookie used during external authentication
            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // check if external login is in the context of an OIDC request
            var context = await this._interaction.GetAuthorizationContextAsync(returnUrl);
            await this._eventService.RaiseAsync(new UserLoginSuccessEvent(info.LoginProvider, info.ProviderKey, user.Id, user.Email, true, context?.Client.ClientId));

            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    // if the client is PKCE then we assume it's native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.RedirectToPage("/Redirect", new { RedirectUrl = returnUrl });
                }
            }

            return this.Redirect(returnUrl);
        }

        private async Task<IActionResult> ProcessWindowsLoginAsync(string providerName, AuthenticationProperties properties, string returnUrl)
        {
            // see if windows auth has already been requested and succeeded
            var result = await this.HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
            if (result?.Principal is WindowsPrincipal wp)
            {
                // Unregistered user deadlock break
                // Force logout user if tried to sign in again with same windows credentials
                if (returnUrl.GetRelativeUrlQueryParam(IdentityServerClaimTypes.UserNotRegistered).LastOrDefault() == "true")
                {
                    var previousUserId = this.HttpContext.Request.Cookies[ProviderConstants.ExternalAuthenticationUser];
                    if (previousUserId.IsPresent() && wp.Identity.Name == previousUserId)
                    {
                        var user = await this._userManager.FindByLoginAsync(providerName, wp.Identity.Name);
                        if (user == null || !user.IsActive)
                        {
                            return this.Challenge(AccountOptions.WindowsAuthenticationSchemeName);
                        }
                    }

                    this.HttpContext.Response.Cookies.Delete(ProviderConstants.ExternalAuthenticationUser);
                }

                // we will issue the external cookie and then redirect the
                // user back to the external callback, in essence, treating windows
                // auth the same as any other external authentication mechanism
                var id = new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.NameIdentifier, wp.Identity.Name) }, AccountOptions.WindowsAuthenticationSchemeName, ClaimTypes.NameIdentifier, string.Empty);

                await this.HttpContext.SignInAsync(IdentityConstants.ExternalScheme, new ClaimsPrincipal(id), properties);
                return this.Redirect(properties.RedirectUri);
            }
            else
            {
                // trigger windows auth
                // since windows auth don't support the redirect uri,
                // this URL is re-triggered when we call challenge
                return this.Challenge(AccountOptions.WindowsAuthenticationSchemeName);
            }
        }

        private void ProcessLoginCallbackForOidc(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            if (externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId) is var sid && sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            if (externalResult.Properties.GetTokenValue("id_token") is var id_token && id_token != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
            }

            if (externalResult.Properties.GetTokenValue("refresh_token") is var refresh_token && refresh_token != null)
            {
                localClaims.Add(new Claim(IdentityServerClaimTypes.ExternalIdpRefreshToken, refresh_token));
            }
        }

        private IActionResult UserNotRegistered(string returnUrl, string userId)
        {
            var queryParams = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseNullableQuery(returnUrl);

            // Used for force logout windows user
            queryParams.Remove(IdentityServerClaimTypes.UserNotRegistered);
            queryParams.Add(IdentityServerClaimTypes.UserNotRegistered, "true");

            returnUrl = queryParams.ToQueryString();

            this.HttpContext.Response.Cookies.Append(ProviderConstants.ExternalAuthenticationUser, userId);

            return this.LoginError(returnUrl, string.Format(LoginErrorConstants.UserNotRegistered, userId));
        }

        private IActionResult LoginError(string returnUrl, string error = null)
        {
            this.ErrorMessage = error ?? LoginErrorConstants.ExternalError;
            return this.RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }
    }
}
