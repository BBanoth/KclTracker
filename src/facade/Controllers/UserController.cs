// <copyright file="UserController.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Facade.Controllers
{
    using System.Threading.Tasks;
    using KclTracker.Services.Application.Users.Commands.CreateUser;
    using KclTracker.Services.Application.Users.Commands.DeleteUser;
    using KclTracker.Services.Application.Users.Commands.Password;
    using KclTracker.Services.Application.Users.Commands.UpdateUser;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : BaseController
    {
        /// <summary>
        /// Inserts or updates user.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST api/v1/user
        ///     {
        ///         "firstName": "sample",
        ///         "lastName": "sample"
        ///         .......
        ///     }.
        /// </remarks>
        /// <param name="command">command.</param>
        /// <returns>id.</returns>
        /// <response code="200">Ok.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<int>> Create([FromBody] CreateUserCommand command)
        {
            return this.Ok(await this.Mediator.Send(command));
        }

        /// <summary>
        /// Inserts or updates user.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     PUT api/v1/user
        ///     {
        ///         "firstName": "sample",
        ///         "lastName": "sample"
        ///         .......
        ///     }.
        /// </remarks>
        /// <param name="command">command.</param>
        /// <returns>Nothing.</returns>
        /// <response code="204">Returns nothing.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateUserCommand command)
        {
            await this.Mediator.Send(command);

            return this.NoContent();
        }

        /// <summary>
        /// Deletes user based on userid.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     DELETE api/v1/user/1.
        /// </remarks>
        /// <param name="id">userId.</param>
        /// <returns>Nothing.</returns>
        /// <response code="204">Returns nothing.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize]
        public async Task<ActionResult> Delete(string id)
        {
            await this.Mediator.Send(new DeleteUserCommand { Id = id });

            return this.NoContent();
        }

        /// <summary>
        /// to reset password.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST api/v1/user/resetpassword.
        /// </remarks>
        /// <param name="command">command.</param>
        /// <returns>Nothing.</returns>
        /// <response code="204">Returns nothing.</response>
        [HttpPost("resetpassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            await this.Mediator.Send(command);

            return this.NoContent();
        }

        /// <summary>
        /// to change password.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST api/v1/user/changepassword.
        /// </remarks>
        /// <param name="command">command.</param>
        /// <returns>Nothing.</returns>
        /// <response code="204">Returns nothing.</response>
        [HttpPost("changepassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            await this.Mediator.Send(command);

            return this.NoContent();
        }
    }
}
