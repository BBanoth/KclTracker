// <copyright file="UpdateUserCommand.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Users.Commands.UpdateUser
{
    using MediatR;

    public class UpdateUserCommand : IRequest
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public bool? IsActive { get; set; }
    }
}
