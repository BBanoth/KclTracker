// <copyright file="ChangePasswordCommandHandler.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Users.Commands.Password
{
    using System.Threading;
    using System.Threading.Tasks;
    using KclTracker.Services.Application.Common.Exceptions.Types;
    using KclTracker.Services.Application.Interfaces;
    using MediatR;
    using Newtonsoft.Json;

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IUserManager _userManager;

        public ChangePasswordCommandHandler(IUserManager userManager)
        {
            this._userManager = userManager;
        }

        public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var response = await this._userManager.ChangePasswordAsync(request.UserId, request.CurrentPassword, request.NewPassword);

            if (!response.Succeeded)
            {
                throw new BadRequestException($"{JsonConvert.SerializeObject(response.Errors)}");
            }

            return Unit.Value;
        }
    }
}
