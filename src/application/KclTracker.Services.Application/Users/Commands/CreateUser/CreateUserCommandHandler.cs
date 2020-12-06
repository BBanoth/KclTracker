// <copyright file="CreateUserCommandHandler.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Users.Commands.CreateUser
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using KclTracker.Services.Application.Interfaces;
    using KclTracker.Services.Domain.Entities;
    using MediatR;
    using Newtonsoft.Json;

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
    {
        private readonly IUserManager _userManager;

        public CreateUserCommandHandler(
            [NotNull] IUserManager userManager)
        {
            this._userManager = userManager;
        }

        public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                UserTypeId = Domain.Enums.UserType.Standard,
                Profile = new UserProfile
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName
                },
                Company = new UserCompany
                {
                    CompanyId = request.CompanyId
                }
            };

            var (response, userId) = await this._userManager.CreateAsync(user, request.Password);

            if (!response.Succeeded)
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    // TODO : Write custom exceptions
                    throw new Exception($"User creation failed: {string.Join(",", JsonConvert.SerializeObject(response.Errors))}");
                }

                // TODO : Write custom exceptions
                throw new Exception($"User created successfully, but the assigning roles is failed: {JsonConvert.SerializeObject(response.Errors)}");
            }

            return userId;
        }
    }
}
