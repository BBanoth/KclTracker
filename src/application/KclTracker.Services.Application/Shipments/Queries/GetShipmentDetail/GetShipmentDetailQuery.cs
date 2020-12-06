using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace KclTracker.Services.Application.Shipments.Queries.GetShipmentDetail
{
    public class GetShipmentDetailQuery : IRequest<ShipmentDetailVm>
    {
        public string DeclarationNumber { get; set; }
    }
}
