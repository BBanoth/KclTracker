// <copyright file="CreateUserCommand.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Users.Commands.CreateUser
{
    using MediatR;

    public class CreateUserCommand : IRequest<string>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public int CompanyId { get; set; }
    }
}
