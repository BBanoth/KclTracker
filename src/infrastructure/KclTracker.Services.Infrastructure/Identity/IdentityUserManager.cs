// <copyright file="IdentityUserManager.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Infrastructure.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using KclTracker.Services.Application.Common;
    using KclTracker.Services.Application.Interfaces;
    using KclTracker.Services.Domain.Entities;
    using KclTracker.Services.Infrastructure.Extensions;
    using KclTracker.Services.Infrastructure.Identity.Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class IdentityUserManager : UserManager<ApplicationUser>, IUserManager
    {
        private readonly ILogger<UserManager<ApplicationUser>> _logger;
        private readonly IMapper _mapper;

        public IdentityUserManager(
            IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<ApplicationUser>> logger,
            IMapper mapper)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            this._logger = logger;
            this._mapper = mapper;
        }

        public async Task<(Result result, string UserId)> CreateAsync(User user, string password = null)
        {
            if (await this.FindByEmailAsync(user.Email) != null)
            {
                this._logger.LogInformation($"User already exists with email {user.Email}");
                return (Result.Failure(new string[] { "User with the provided email is already registered" }), string.Empty);
            }

            if (await this.FindByNameAsync(user.UserName) != null)
            {
                this._logger.LogInformation($"User already exists with username {user.UserName}");
                return (Result.Failure(new string[] { "User with the provided username is already registered" }), string.Empty);
            }

            var entity = this._mapper.Map<ApplicationUser>(user);

            IdentityResult result;

            if (!string.IsNullOrEmpty(password))
            {
                result = await this.CreateAsync(entity, password);
            }
            else
            {
                result = await this.CreateAsync(entity);
            }

            return (result.ToApplicationResult(), entity.Id);
        }

        public async Task<Result> DeleteAsync(string id)
        {
            var user = await this.FindByIdAsync(id);

            if (user != null)
            {
                var result = await this.DeleteAsync(user);

                this._logger.LogInformation($"User with email {user.Email} deleted");

                return result.ToApplicationResult();
            }

            return Result.Failure(new string[] { "User does not exists with the provided user id" });
        }

        public IQueryable<User> GetAllAsync(int pageIndex, int pageSize)
        {
            return this.Users.AsNoTracking().OrderBy(x => x.Profile.FirstName)
                   .Skip(pageIndex * pageSize).Take(pageSize)
                   .ProjectTo<User>(this._mapper.ConfigurationProvider);
        }

        public IQueryable<User> GetDetailAsync(string id)
        {
            return this.Users.AsNoTracking()
               .Where(user => user.Id == id)
               .Include(user => user.Profile)
               .Include(user => user.Company)
               .ProjectTo<User>(this._mapper.ConfigurationProvider);
        }

        public async Task<Result> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            var userEntity = await this.Users
                .Include(user => user.Profile)
                .Where(u => u.Id == user.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (userEntity != null)
            {
                if (userEntity.Profile == null)
                {
                    userEntity.Profile = new ApplicationUserProfile
                    {
                        FirstName = user.Profile.FirstName,
                        LastName = user.Profile.LastName
                    };
                }
                else
                {
                    userEntity.Profile.FirstName = user.Profile.FirstName;
                    userEntity.Profile.LastName = user.Profile.LastName;
                }

                userEntity.PhoneNumber = user.PhoneNumber;

                userEntity.IsActive = user.IsActive;

                var result = await this.UpdateAsync(userEntity);

                return result.ToApplicationResult();
            }

            return Result.Failure(new string[] { "User does not exists with the provided user id" });
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string userId)
        {
            var user = await this.FindByIdAsync(userId);

            if (user == null)
            {
                return string.Empty;
            }

            return await this.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<Result> ResetPasswordAsync(string email, string token, string password)
        {
            var user = await this.FindByEmailAsync(email);

            if (user == null)
            {
                return Result.Failure(new string[] { "User not exists with the provided email" });
            }

            var result = await this.ResetPasswordAsync(user, token, password);

            return result.ToApplicationResult();
        }

        public async Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await this.FindByIdAsync(userId);

            if (user == null)
            {
                return Result.Failure(new string[] { "User not exists with the provided user id" });
            }

            var result = await this.ChangePasswordAsync(user, currentPassword, newPassword);

            return result.ToApplicationResult();
        }

        public Task<int> GetUsersCountAsync()
        {
            return this.Users.CountAsync();
        }
    }
}
