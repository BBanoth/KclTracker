// <copyright file="UriExtensions.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Extensions
{
    using System;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Primitives;

    public static class UriExtensions
    {
        public static string GetHost(this string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                var uri = new Uri(url);

                return uri.GetLeftPart(UriPartial.Authority);
            }

            return null;
        }

        public static StringValues GetRelativeUrlQueryParam(this string url, string key)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                var queryIndex = url.IndexOf('?');
                var queryUrl = queryIndex > -1 ? url.Substring(queryIndex) : string.Empty;

                if (!string.IsNullOrWhiteSpace(queryUrl))
                {
                    var queryParams = QueryHelpers.ParseNullableQuery(queryUrl);
                    if (queryParams != null && queryParams.TryGetValue(key, out var values) && !StringValues.IsNullOrEmpty(values))
                    {
                        return values;
                    }
                }
            }

            return string.Empty;
        }

        public static StringValues GetQueryParam(this string url, string key)
        {
            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
            {
                var queryParams = QueryHelpers.ParseNullableQuery(uri?.Query);
                if (queryParams != null && queryParams.TryGetValue(key, out var values) && !StringValues.IsNullOrEmpty(values))
                {
                    return values;
                }
            }

            return string.Empty;
        }
    }
}
