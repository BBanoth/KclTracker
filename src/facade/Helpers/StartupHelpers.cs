// <copyright file="StartupHelpers.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Facade.Helpers
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using IdentityServer4;
    using KclTracker.Services.Application.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.Swagger;

    public static class StartupHelpers
    {
        public static IServiceCollection AddKclSwagger([NotNull] this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.ToString());

                // Adds fluent validation rules to swagger
                options.AddFluentValidationRules();

                // XML description comments
                foreach (var xmlPath in GetXmlDocuments())
                {
                    options.IncludeXmlComments(xmlPath);
                }

                options.SwaggerDoc("v1", configuration.GetSection("OpenApi").Get<OpenApiInfo>());

                // Adds a security scheme
                options.AddSecurityDefinition("Bearer", configuration.GetSection("OpenApi:SecurityScheme").Get<OpenApiSecurityScheme>());

                // Applies the security scheme globally
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] { }
                    }
                });
            });

            services.AddSwaggerGenNewtonsoftSupport();

            return services;
        }

        public static IServiceCollection AddAuthenticationAndAuthorization([NotNull] this IServiceCollection services)
        {
            // Adding Local Api Authentication
            services.AddAuthentication()
                .AddLocalApi(options =>
                {
                    options.ExpectedScope = IdentityServerConstants.LocalApi.ScopeName;
                });

            // Adding Local Api Authorization
            services.AddAuthorization(options =>
            {
                var localApiPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

                // Basic user authentication policy i.e Require authenticated user
                options.AddPolicy(IdentityServerConstants.LocalApi.PolicyName, localApiPolicy);
            });

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }

        public static IApplicationBuilder UseKclSwagger([NotNull] this IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kcl v1 Api");
                c.OAuthClientId("swagger-ui");
                c.OAuthAppName("Swagger UI");
                c.OAuthUsePkce();
            });

            app.UseReDoc(options =>
            {
                options.SpecUrl = "/swagger/v1/swagger.json";
                options.RoutePrefix = "docs";
                options.DocumentTitle = "Kcl v1 Api";
            });

            return app;
        }

        private static string[] GetXmlDocuments()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            return currentAssembly.GetReferencedAssemblies()
                .Union(new AssemblyName[] { currentAssembly.GetName() })
                .Select(assembly => Path.Combine(Path.GetDirectoryName(currentAssembly.Location), $"{assembly.Name}.xml"))
                .Where(file => File.Exists(file)).ToArray();
        }
    }
}
