// <copyright file="ResetPasswordCommandHandler.cs" company="Agility E Services">
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

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
    {
        private readonly IUserManager _userManager;

        public ResetPasswordCommandHandler(IUserManager userManager)
        {
            this._userManager = userManager;
        }

        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var response = await this._userManager.ResetPasswordAsync(request.Email, request.Token, request.Password);

            if (!response.Succeeded)
            {
                throw new BadRequestException($"{JsonConvert.SerializeObject(response.Errors)}");
            }

            return Unit.Value;
        }
    }
}
