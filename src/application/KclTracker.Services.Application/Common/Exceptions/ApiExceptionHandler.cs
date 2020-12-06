// <copyright file="ApiExceptionHandler.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Common.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using KclTracker.Services.Application.Common.Exceptions.Types;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class ApiExceptionHandler
    {
        private readonly ILogger<ApiExceptionHandler> _logger;
        private readonly IDictionary<Type, Func<Exception, ExceptionResult>> _exceptionHandlers;

        public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger)
        {
            this._logger = logger;

            this._exceptionHandlers = new Dictionary<Type, Func<Exception, ExceptionResult>>
            {
                { typeof(ValidationException), this.HandleValidationException },
                { typeof(BadRequestException), this.HandleBadRequestException },
                { typeof(ForbiddenException), this.HandleForbiddenException },
                { typeof(NotFoundException), this.HandleNotFoundException },
                { typeof(UnauthorizedException), this.HandleUnauthorizedException }
            };
        }

        public ExceptionResult Handle(Exception exception)
        {
            this._logger.LogError($"Exception log: {JsonConvert.SerializeObject(exception)}");

            Type type = exception.GetType();
            if (this._exceptionHandlers.ContainsKey(type))
            {
                return this._exceptionHandlers[type].Invoke(exception);
            }

            return this.HandleUnknownException(exception);
        }

        private ExceptionResult HandleValidationException(Exception exception)
        {
            var validationException = exception as ValidationException;

            if (validationException.Errors.Any())
            {
                return new ExceptionResult(HttpStatusCode.BadRequest, validationException.Errors);
            }

            return new ExceptionResult(HttpStatusCode.BadRequest, validationException.Message);
        }

        private ExceptionResult HandleBadRequestException(Exception exception)
        {
            var badrequestException = exception as BadRequestException;

            return new ExceptionResult(HttpStatusCode.BadRequest, badrequestException.Message);
        }

        private ExceptionResult HandleForbiddenException(Exception exception)
        {
            var forbiddenException = exception as ForbiddenException;

            return new ExceptionResult(HttpStatusCode.Forbidden, forbiddenException.Message);
        }

        private ExceptionResult HandleNotFoundException(Exception exception)
        {
            var notFoundException = exception as NotFoundException;

            return new ExceptionResult(HttpStatusCode.NotFound, notFoundException.Message);
        }

        private ExceptionResult HandleUnauthorizedException(Exception exception)
        {
            var unauthorizedException = exception as UnauthorizedException;

            return new ExceptionResult(HttpStatusCode.Unauthorized, unauthorizedException.Message);
        }

        private ExceptionResult HandleUnknownException(Exception exception)
        {
            if (exception is UnauthorizedAccessException)
            {
                return new ExceptionResult(HttpStatusCode.Unauthorized, exception.Message);
            }

            if (exception is MemberAccessException)
            {
                return new ExceptionResult(HttpStatusCode.Forbidden, exception.Message);
            }

            return new ExceptionResult(HttpStatusCode.InternalServerError, exception.Message);
        }
    }
}
