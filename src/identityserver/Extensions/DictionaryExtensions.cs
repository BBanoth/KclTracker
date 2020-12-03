// <copyright file="DictionaryExtensions.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Primitives;

    public static class DictionaryExtensions
    {
        public static bool Validate(this IDictionary<string, string> data, string property)
        {
            return data != null && data.TryGetValue(property, out string value) && value.IsPresent() && bool.TryParse(value, out bool result) && result;
        }

        public static string GetValue(this IDictionary<string, string> data, string property)
        {
            if (data != null && data.TryGetValue(property, out string value) && value.IsPresent())
            {
                return value;
            }

            return default;
        }

        public static string ToQueryString(this IDictionary<string, StringValues> parameters)
        {
            return string.Join("&", parameters.Select(x => $"{x.Key}={x.Value}"));
        }
    }
}
