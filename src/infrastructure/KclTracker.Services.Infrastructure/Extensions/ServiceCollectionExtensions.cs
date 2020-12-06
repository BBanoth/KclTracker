// <copyright file="ServiceCollectionExtensions.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Extensions
{
    using System;
    using System.Reflection;
    using AutoMapper;
    using KclTracker.Services.Application.Interfaces;
    using KclTracker.Services.Infrastructure.Identity;
    using KclTracker.Services.Infrastructure.Identity.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddDbContext<UserIdentityDbContext>(options =>
            {
                options.EnableSensitiveDataLogging();
                options.UseSqlServer(configuration.GetConnectionString("KclTracker"));
            });

            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddUserManager<IdentityUserManager>()
                .AddEntityFrameworkStores<UserIdentityDbContext>();

                // .AddDefaultTokenProviders();
            services.AddScoped<IUserManager, IdentityUserManager>();

            return services;
        }
    }
}
