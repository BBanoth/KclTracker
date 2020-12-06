// <copyright file="ClaimModel.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Interfaces
{
    using System;

    [Serializable]
    public class ClaimModel
    {
        public ClaimModel(string type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        public string Type { get; set; }

        public string Value { get; set; }
    }
}
