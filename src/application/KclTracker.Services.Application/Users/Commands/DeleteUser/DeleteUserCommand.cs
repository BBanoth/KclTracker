// <copyright file="DeleteUserCommand.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Users.Commands.DeleteUser
{
    using MediatR;

    public class DeleteUserCommand : IRequest
    {
        public string Id { get; set; }
    }
}
