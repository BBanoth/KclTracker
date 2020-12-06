// <copyright file="GetCompanyListQuery.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Companies.Queries
{
    using System.Collections.Generic;
    using MediatR;

    public class GetCompanyListQuery : IRequest<IEnumerable<CompanyListVm>>
    {
        public string SearchText { get; set; }
    }
}
