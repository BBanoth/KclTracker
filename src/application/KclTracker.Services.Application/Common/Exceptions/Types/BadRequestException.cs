// <copyright file="BadRequestException.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Common.Exceptions.Types
{
    using System;

    public class BadRequestException : Exception
    {
        private const string DefaultMessage = "Bad request";

        public BadRequestException(string message = DefaultMessage)
              : base(message)
        {
        }

        public BadRequestException(Exception ex, string message = DefaultMessage)
             : base(message, ex)
        {
        }
    }
}
