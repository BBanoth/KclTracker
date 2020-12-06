// <copyright file="ResetPasswordCommand.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Users.Commands.Password
{
    using MediatR;

    public class ResetPasswordCommand : IRequest
    {
        public string Email { get; set; }

        public string Token { get; set; }

        public string Password { get; set; }
    }
}
