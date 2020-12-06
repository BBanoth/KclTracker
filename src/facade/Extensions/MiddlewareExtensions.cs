// <copyright file="MiddlewareExtensions.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Facade.Extensions
{
    using KclTracker.Services.Application.Common.Exceptions;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    public static class MiddlewareExtensions
    {
        public static RequestDelegate ExceptionTerminalMiddlewareDelegate
        {
            get
            {
                return new RequestDelegate(async context =>
                {
                    var exceptionInfo = context.Features.Get<IExceptionHandlerFeature>();
                    var exceptionResult = context.RequestServices.GetRequiredService<ApiExceptionHandler>().Handle(exceptionInfo.Error);

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)exceptionResult.StatusCode;
                    await context.Response.WriteAsync(exceptionResult.Message);
                });
            }
        }
    }
}
