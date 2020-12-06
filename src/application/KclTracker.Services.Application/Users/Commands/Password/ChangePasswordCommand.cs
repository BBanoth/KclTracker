// <copyright file="ChangePasswordCommand.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Users.Commands.Password
{
    using MediatR;

    public class ChangePasswordCommand : IRequest
    {
        public string UserId { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
