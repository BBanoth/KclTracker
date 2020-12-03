// <copyright file="LoginErrorConstants.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Constants
{
    public static class LoginErrorConstants
    {
        public const string UserNotRegistered = "User {0} is either not registered with us or not active. Please contact support";

        public const string ExternalError = "External login error";

        public const string ExternalRemoteError = "Error from external provider: {0}";
    }
}
