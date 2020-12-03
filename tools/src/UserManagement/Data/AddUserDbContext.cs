// <copyright file="AddUserDbContext.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Tools.UserManagement.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class AddUserDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AddUserDbContext(DbContextOptions<AddUserDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("security");

            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasOne(user => user.Profile)
                .WithOne()
                .HasForeignKey<UserProfile>(profile => profile.UserId);

            builder.Entity<ApplicationUser>()
                .HasMany(user => user.UserLogins)
                .WithOne()
                .HasForeignKey(login => login.UserId);

            builder.Entity<ApplicationUser>()
                .Property(user => user.UserTypeId)
                .HasConversion<int>();

            builder.Entity<ApplicationRole>()
                .Property(user => user.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<ApplicationRole>()
                .HasMany(role => role.Claims)
                .WithOne()
                .HasForeignKey(roleClaim => roleClaim.RoleId);

            builder.Entity<IdentityRoleClaim<string>>()
                .Property(roleClaim => roleClaim.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
