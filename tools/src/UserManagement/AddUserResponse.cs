// <copyright file="AddUserResponse.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Tools.UserManagement
{
    using KclTracker.Tools.UserManagement.Data;

    public class AddUserResponse
    {
        public bool EmailExists { get; set; }

        public bool DomainUserExists { get; set; }

        public bool PasswordAttemptsExceeded { get; set; }

        public bool AddedUser { get; set; }

        public bool AddedAdminRole { get; set; }

        public ApplicationUser User { get; set; }
    }
}
