// <copyright file="Startup.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Facade
{
    using FluentValidation.AspNetCore;
    using IdentityServer4;
    using KclTracker.IdentityServer.Extensions;
    using KclTracker.Services.Application.Extensions;
    using KclTracker.Services.Facade.Extensions;
    using KclTracker.Services.Facade.Helpers;
    using KclTracker.Services.Infrastructure.Extensions;
    using KclTracker.Services.Infrastructure.Persistence.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json;
    using Serilog;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseDeveloperPageExceptionFilter();

            var mvcBuilder = services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            });

            // Adds fluent validation
            mvcBuilder.AddFluentValidation();

            // IdentityServer Library
            services.AddKclIdentityServer(this.Configuration);

            // Application
            services.AddApplication(this.Configuration);

            // Infrastructure
            services.AddInfrastructure(this.Configuration);

            // Persistence
            services.AddPersistence(this.Configuration);

            // Authentication and Authorization
            services.AddAuthenticationAndAuthorization();

            // Swagger
            services.AddKclSwagger(this.Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseExceptionHandler(appBuilder => appBuilder.Run(MiddlewareExtensions.ExceptionTerminalMiddlewareDelegate));

            // Swagger
            app.UseKclSwagger();

            // Cors
            app.UseCors(policyBuilder =>
            {
                if (this.Configuration.GetSection("Cors:OriginsWithCredentials").Exists())
                {
                    policyBuilder
                        .WithOrigins(this.Configuration.GetSection("Cors:OriginsWithCredentials").Get<string[]>())
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }

                if (this.Configuration.GetSection("Cors:Origins").Exists())
                {
                    policyBuilder
                            .WithOrigins(this.Configuration.GetSection("Cors:Origins").Get<string[]>())
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                }
            });

            app.UseRouting();

            app.UseCookiePolicy();

            app.UseKclIdentityServer();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization(IdentityServerConstants.LocalApi.PolicyName);
                endpoints.MapRazorPages();
            });
        }
    }
}
