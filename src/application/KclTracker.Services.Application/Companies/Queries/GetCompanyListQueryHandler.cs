// <copyright file="GetCompanyListQueryHandler.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Companies.Queries
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using KclTracker.Services.Application.Interfaces;
    using MediatR;
    using Microsoft.EntityFrameworkCore;

    public class GetCompanyListQueryHandler : IRequestHandler<GetCompanyListQuery, IEnumerable<CompanyListVm>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetCompanyListQueryHandler([NotNull] IApplicationDbContext context, [NotNull] IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<CompanyListVm>> Handle(GetCompanyListQuery request, CancellationToken cancellationToken)
        {
            return await this._context.Companies.AsNoTracking()
                .Where(x => x.Name.Contains(request.SearchText))
                .ProjectTo<CompanyListVm>(this._mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}
