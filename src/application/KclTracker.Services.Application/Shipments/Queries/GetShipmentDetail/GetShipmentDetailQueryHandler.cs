using KclTracker.Services.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KclTracker.Services.Application.Shipments.Queries.GetShipmentDetail
{
    public class GetShipmentDetailQueryHandler : IRequestHandler<GetShipmentDetailQuery, ShipmentDetailVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _userService;

        public GetShipmentDetailQueryHandler([NotNull] IApplicationDbContext context, [NotNull] ICurrentUserService userService)
        {
            this._context = context;
            this._userService = userService;
        }

        public Task<ShipmentDetailVm> Handle(GetShipmentDetailQuery request, CancellationToken cancellationToken)
        {
            var companyQuery = from company in this._context.UserCompanies
                               where company.UserId == this._userService.UserId && company.IsActive && !company.IsDeleted
                               select company;

            var query = from company in companyQuery
                        join shipment in this._context.Shipments.Include(x => x.InspectionType).Include(x => x.History).ThenInclude(x => x.Status)
                        on company.CompanyId equals shipment.CompanyId
                        where shipment.DeclarationNumber == request.DeclarationNumber
                        select new ShipmentDetailVm
                        {
                            CdnNumber = shipment.CdnNumber,
                            ContainerNumbers = shipment.ContainerNumbers,
                            DateOfEntry = shipment.DateOfEntry,
                            DeclarationNumber = shipment.DeclarationNumber,
                            InspectionType = shipment.InspectionType.Name,
                            NumberOfContainers = shipment.NumberOfContainers,
                            PartName = shipment.PartName,
                            History = shipment.History.OrderByDescending(x => x.Status.OrderId).Select(x => new Common.Models.ShipmentHistoryModel
                            {
                                Status = x.Status.Name,
                                OrderId = x.Status.OrderId,
                                UpdatedOn = x.UpdatedOn
                            })
                        };

            return query.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
