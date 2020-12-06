// <copyright file="ExceptionResult.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Common.Exceptions
{
    using System.Net;
    using Newtonsoft.Json;

    public class ExceptionResult
    {
        public ExceptionResult(HttpStatusCode code, string message)
        {
            this.StatusCode = code;
            this.Message = message;
        }

        public ExceptionResult(HttpStatusCode code, object errors)
        {
            this.StatusCode = code;
            this.Message = JsonConvert.SerializeObject(errors);
        }

        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; }
    }
}
