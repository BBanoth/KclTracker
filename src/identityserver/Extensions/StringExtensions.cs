// <copyright file="StringExtensions.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Extensions
{
    public static class StringExtensions
    {
        public static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static bool IsMissing(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}
