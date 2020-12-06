// <copyright file="UnauthorizedException.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Common.Exceptions.Types
{
    using System;

    public class UnauthorizedException : Exception
    {
        private const string DeafultMessage = "Unauthorized";

        public UnauthorizedException()
             : base(DeafultMessage)
        {
        }

        public UnauthorizedException(string message)
              : base(message)
        {
        }
    }
}
