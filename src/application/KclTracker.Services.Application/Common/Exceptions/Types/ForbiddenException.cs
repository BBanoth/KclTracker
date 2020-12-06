// <copyright file="ForbiddenException.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Common.Exceptions.Types
{
    using System;

    public class ForbiddenException : Exception
    {
        private const string DeafultMessage = "Access is forbidden";

        public ForbiddenException()
             : base(DeafultMessage)
        {
        }

        public ForbiddenException(string message)
              : base(message)
        {
        }
    }
}
