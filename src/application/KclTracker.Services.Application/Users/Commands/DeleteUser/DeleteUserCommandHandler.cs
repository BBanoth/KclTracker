// <copyright file="DeleteUserCommandHandler.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Users.Commands.DeleteUser
{
    using System.Threading;
    using System.Threading.Tasks;
    using KclTracker.Services.Application.Common.Exceptions.Types;
    using KclTracker.Services.Application.Interfaces;
    using MediatR;

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserManager _userManager;
        private readonly ICurrentUserService _userService;

        public DeleteUserCommandHandler(IUserManager userManager, ICurrentUserService currentUserService)
        {
            this._userManager = userManager;
            this._userService = currentUserService;
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == this._userService.UserId)
            {
                throw new BadRequestException("Cannot delete own account");
            }

            await this._userManager.DeleteAsync(request.Id);

            return Unit.Value;
        }
    }
}
