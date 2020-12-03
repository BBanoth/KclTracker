// <copyright file="ConfigurationExtensions.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Extensions
{
    using Microsoft.Extensions.Configuration;

    public static class ConfigurationExtensions
    {
        public static string GetConnectionString(this IConfiguration configuration)
        {
            return configuration.GetConnectionString("KclTracker");
        }

        public static IConfiguration GetOperationalStoreOptions(this IConfiguration configuration)
        {
            return configuration.GetSection("IdentityServer:OperationalStoreOptions");
        }
    }
}
