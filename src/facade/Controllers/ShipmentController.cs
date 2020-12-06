// <copyright file="CompanyController.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Facade.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KclTracker.Services.Application.Companies.Queries;
    using KclTracker.Services.Application.Shipments.Queries.GetShipmentDetail;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class ShipmentController : BaseController
    {
        /// <summary>
        /// Gets shipment details by query.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST api/v1/shipment.
        /// </remarks>
        /// <param name="query">query.</param>
        /// <returns>Shipment details.</returns>
        /// <response code="200">Returns shipment details.</response>
        [HttpPost("details")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ShipmentDetailVm), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(GetShipmentDetailQuery query)
        {
            return this.Ok(await this.Mediator.Send(query));
        }
    }
}
