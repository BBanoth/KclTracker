// <copyright file="LoginInputModel.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.IdentityServer.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
