// <copyright file="IUserManager.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Services.Application.Interfaces
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KclTracker.Services.Application.Common;
    using KclTracker.Services.Domain.Entities;

    public interface IUserManager
    {
        Task<(Result result, string UserId)> CreateAsync(User user, string password = null);

        IQueryable<User> GetAllAsync(int pageIndex, int pageSize);

        IQueryable<User> GetDetailAsync(string id);

        Task<Result> UpdateAsync(User user, CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(string id);

        Task<string> GeneratePasswordResetTokenAsync(string userId);

        Task<Result> ResetPasswordAsync(string email, string token, string password);

        Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

        Task<int> GetUsersCountAsync();
    }
}
