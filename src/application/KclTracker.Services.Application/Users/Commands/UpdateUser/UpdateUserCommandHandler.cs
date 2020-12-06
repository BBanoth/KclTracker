// <copyright file="UpdateUserCommandHandler.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Users.Commands.UpdateUser
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using KclTracker.Services.Application.Interfaces;
    using KclTracker.Services.Domain.Entities;
    using MediatR;
    using Newtonsoft.Json;

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
    {
        private readonly IUserManager _userManager;

        public UpdateUserCommandHandler(IUserManager userManager)
        {
            this._userManager = userManager;
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = request.Id,
                PhoneNumber = request.PhoneNumber,
                Profile = new UserProfile { FirstName = request.FirstName, LastName = request.LastName }
            };

            if (request.IsActive.HasValue)
            {
                user.IsActive = request.IsActive.Value;
            }

            var result = await this._userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception($"User update failed: {JsonConvert.SerializeObject(result.Errors)}");
            }

            return Unit.Value;
        }
    }
}
