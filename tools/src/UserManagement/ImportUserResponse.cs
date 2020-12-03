// <copyright file="ImportUserResponse.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Tools.UserManagement
{
    internal class ImportUserResponse
    {
        public string Data { get; set; }

        public bool IsUserAdded { get; set; }

        public bool IsRoleAssigned { get; set; }
    }
}
