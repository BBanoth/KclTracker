// <copyright file="ServiceCollectionExtensions.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Extensions
{
    using System;
    using IdentityServer4.EntityFramework.Options;
    using KclTracker.IdentityServer.Configurations;
    using KclTracker.IdentityServer.Data;
    using KclTracker.IdentityServer.Data.Entities;
    using KclTracker.IdentityServer.Services;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKclIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Chrome 80 and Safari 12 SamSite Breaking Changes Port
            services.ConfigureNonBreakingSameSiteCookies();

            services.AddDbContext<UserIdentityDbContext>(options => options.UseSqlServer(configuration.GetConnectionString()));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Stores.MaxLengthForKeys = 128;
            })

            // Used while creating user principal
            // UserClaimsPrincipalFactory<TUser> gets user claims which impacts cookie size
            // UserClaimsPrincipalFactory<TUser, TRole> gets user claims and also role claims which impacts cookie size
            // This is to avoid including claims to identity cookie
            .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory<ApplicationUser>>()
            .AddEntityFrameworkStores<UserIdentityDbContext>();

            services.Configure<IISServerOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            // IdentityServer configuration
            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // Set no cache for .well-known configurations
                options.Discovery.ResponseCacheInterval = 0;
            })
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(configuration.GetSection("IdentityServer:IdentityResources"))
                .AddInMemoryClients(configuration.GetSection("IdentityServer:Clients"))
                .AddInMemoryApiResources(configuration.GetSection("IdentityServer:ApiResources"))
                .AddInMemoryApiScopes(configuration.GetSection("IdentityServer:ApiScopes"))
                .AddOperationalStore<IdentityServerDbContext>(options =>
                {
                    var optionsConfig = configuration.GetOperationalStoreOptions().Get<OperationalStoreOptions>();

                    options.EnableTokenCleanup = optionsConfig?.EnableTokenCleanup ?? options.EnableTokenCleanup;
                    options.TokenCleanupInterval = optionsConfig?.TokenCleanupInterval ?? options.TokenCleanupInterval;
                    options.TokenCleanupBatchSize = optionsConfig?.TokenCleanupBatchSize ?? options.TokenCleanupBatchSize;

                    options.DefaultSchema = "idp";
                    options.ConfigureDbContext = (dbOptionsBuilder) => dbOptionsBuilder.UseSqlServer(configuration.GetConnectionString());
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<CustomProfileService>();

            // SameSite cookies deletion on logout
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<CookieAuthenticationOptions>, CustomPostConfigureCookieAuthenticationOptions>());

            services.AddRazorPages();

            return services;
        }
    }
}
