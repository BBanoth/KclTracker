// <copyright file="ApplicationBuilderExtensions.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Extensions
{
    using Microsoft.AspNetCore.Builder;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseIdentity(this IApplicationBuilder app)
        {
            app.UseIdentityServer();

            return app;
        }
    }
}
