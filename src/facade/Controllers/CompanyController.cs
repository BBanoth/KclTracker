// <copyright file="CompanyController.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Facade.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KclTracker.Services.Application.Companies.Queries;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/v1/[controller]")]
    public class CompanyController : BaseController
    {
        /// <summary>
        /// Gets company list by name.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     GET api/v1/company/search/name.
        /// </remarks>
        /// <param name="name">Company Name.</param>
        /// <returns>Company name with id.</returns>
        /// <response code="200">Returns company name with id.</response>
        [HttpGet("search/{name}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<CompanyListVm>), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string name)
        {
            return this.Ok(await this.Mediator.Send(new GetCompanyListQuery { SearchText = name }));
        }
    }
}
