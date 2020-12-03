// <copyright file="UserService.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Tools.UserManagement
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ConsoleTables;
    using KclTracker.Tools.UserManagement.Constants;
    using KclTracker.Tools.UserManagement.Data;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Serilog;

    public class UserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IPasswordValidator<ApplicationUser> _passwordValidator;
        private readonly AddUserDbContext _dbContext;
        private bool retryUserCreation = false;

        public UserService(IServiceProvider provider)
        {
            this._userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            this._roleManager = provider.GetRequiredService<RoleManager<ApplicationRole>>();
            this._passwordValidator = provider.GetRequiredService<IPasswordValidator<ApplicationUser>>();
            this._dbContext = provider.GetRequiredService<AddUserDbContext>();
        }

        public async Task AddSysAdminRoleAsync()
        {
            try
            {
                var result = await this.CreateSysAdminRoleAsync();
                if (result.Succeeded)
                {
                    Log.Information("SysAdmin role added");
                    Console.Write("\n************************************************************\n");
                    Console.Write("SysAdmin role is added to the system");
                    Console.Write("\n************************************************************\n\n");
                }
                else
                {
                    Log.Information($"SysAdmin role creation failed, \n Errors: {string.Join(',', result.Errors)})");
                    Console.Write("\n************************************************************\n");
                    Console.Write("An error occured while creating sysadmin role");
                    Console.Write("\n************************************************************\n\n");
                }
            }
            catch (Exception ex)
            {
                Log.Information($"SysAdmin role creation failed, \n Error: {JsonConvert.SerializeObject(ex)}");
                Console.Write("\n************************************************************\n");
                Console.Write("An error occured while creating sysadmin role");
                Console.Write("\n************************************************************\n\n");
            }
        }

        public async Task AddUserAsync(WorkOption selectedOption)
        {
            Console.WriteLine("Please provide the required information to add the user");

            bool isAdmin = selectedOption == WorkOption.AddAdminUser || selectedOption == WorkOption.AddDomainAdminUser;
            bool isDomainUser = selectedOption == WorkOption.AddDomainUser || selectedOption == WorkOption.AddDomainAdminUser;

            do
            {
                this.retryUserCreation = false;

                var response = await this.CreateUser(isAdmin, isDomainUser);

                string message = null;

                if (!response.AddedUser)
                {
                    // Email already registered case
                    if (response.EmailExists)
                    {
                        message = "Email already exists. Do you want to try with another ?\n";
                    }

                    // Domain user already registered case
                    if (response.DomainUserExists)
                    {
                        message = "Domain user already exists. Do you want to retry adding user ?\n";
                    }

                    // Invalid password entered max times
                    if (response.PasswordAttemptsExceeded)
                    {
                        message = "Unable to get the password. Do you want to retry adding user ?\n";
                    }

                    message ??= "User creation failed, Do you want to retry ?\n";

                    Console.Write("\n************************************************************\n");
                    Console.Write(message);
                    Console.Write("\n************************************************************\n\n");

                    if (this.GetConfirmation())
                    {
                        this.retryUserCreation = true;
                    }
                }
                else
                {
                    if (isAdmin && !response.AddedAdminRole)
                    {
                        message = "User added successfully but the admin role assigning failed.\nPlease check whether the role exists in the system or not.";
                    }
                    else if (response.AddedAdminRole)
                    {
                        message = "User with SysAdmin role added successfully";
                    }
                    else
                    {
                        message = "User added successfully";
                    }

                    Console.Write("\n************************************************************\n");
                    Console.Write(message);
                    Console.Write("\n************************************************************\n\n");
                }
            }
            while (this.retryUserCreation);
        }

        public async Task<IdentityResult> AddUserAsync(ApplicationUser user, string password = null)
        {
            if (!string.IsNullOrWhiteSpace(password))
            {
                return await this._userManager.CreateAsync(user, password);
            }
            else
            {
                return await this._userManager.CreateAsync(user);
            }
        }

        public async Task<IdentityResult> AssignSysAdminRoleAsync(string email)
        {
            var user = await this._userManager.FindByEmailAsync(email);

            return await this._userManager.AddToRoleAsync(user, IdentityConstants.SysAdminRole);
        }

        public async Task<IdentityResult> CreateSysAdminRoleAsync()
        {
            if (await this._roleManager.RoleExistsAsync(IdentityConstants.SysAdminRole))
            {
                throw new InvalidOperationException("SysAdmin role already exists");
            }

            var role = new ApplicationRole
            {
                Name = IdentityConstants.SysAdminRole
            };

            return await this._roleManager.CreateAsync(role);
        }

        public async Task ImportDomainUsersFromFileAsync()
        {
            var userDataLines = await File.ReadAllLinesAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DomainUsersData.txt"));

            var importUsersResponse = new List<ImportUserResponse>();

            foreach (var userDataLine in userDataLines)
            {
                var userResponse = new ImportUserResponse { Data = userDataLine };
                importUsersResponse.Add(userResponse);

                var userData = userDataLine.Split(',');
                if (userData.Length != 6)
                {
                    Log.Error($"Data is not in correct format: {userDataLine}");
                    continue;
                }

                var email = userData.ElementAt(0);
                bool emailExists = (await this._userManager.FindByEmailAsync(email)) != null;
                if (emailExists)
                {
                    Log.Error($"User already exits with the email {email}, Data: {userDataLine}");
                    continue;
                }

                if (!Enum.TryParse<UserType>(userData.ElementAt(1), out var userType))
                {
                    Log.Error($"User type is not valid, Data: {userDataLine}");
                    continue;
                }

                var userName = userData.ElementAt(2);

                if (userName.IndexOf("\\") < 0)
                {
                    Log.Error($"User name {userName} is not valid, Data: {userDataLine}");
                    continue;
                }

                if (await this._userManager.FindByLoginAsync(IdentityConstants.WindowsLoginProvider, userName) != null)
                {
                    Log.Error($"Domain user already exits with username {userName}, Data: {userDataLine}");
                    continue;
                }

                var user = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    UserTypeId = userType,
                    IsActive = true,
                    Profile = new UserProfile
                    {
                        FirstName = userData.ElementAt(3),
                        LastName = userData.ElementAt(4)
                    }
                };

                user.UserLogins.Add(new IdentityUserLogin<string>
                {
                    LoginProvider = IdentityConstants.WindowsLoginProvider,
                    ProviderDisplayName = IdentityConstants.WindowsLoginProviderDisplayName,
                    ProviderKey = userName
                });

                var result = await this._userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    Log.Error($"User creation failed with data: {userDataLine}");
                    continue;
                }

                userResponse.IsUserAdded = true;

                Log.Information($"User created successfully with data: {userDataLine}");

                var role = userData.ElementAt(4);
                try
                {
                    result = await this._userManager.AddToRoleAsync(user, role);
                    if (!result.Succeeded)
                    {
                        Log.Error($"Adding role {role} to user is failed, Data: {userDataLine}");
                        continue;
                    }

                    userResponse.IsRoleAssigned = true;

                    Log.Information($"Added role {role} to user, Data: {userDataLine}");
                }
                catch (Exception ex)
                {
                    Log.Error($"An error occurred while adding role {role} to the user, Data: {userDataLine}\nException: {JsonConvert.SerializeObject(ex)}");
                    continue;
                }
            }

            if (importUsersResponse.Any())
            {
                ConsoleTable
                    .From(importUsersResponse)
                    .Configure(o => o.NumberAlignment = Alignment.Right)
                    .Write();
            }
            else
            {
                Console.Write("\n************************************************************\n");
                Console.Write("File does not contain any data");
                Console.Write("\n************************************************************\n\n");
            }
        }

        private async Task<AddUserResponse> CreateUser(bool isAdmin = false, bool isDomainUser = false)
        {
            var user = new ApplicationUser { Profile = new UserProfile(), UserTypeId = isAdmin ? UserType.Admin : UserType.Standard };

            Console.Write("Email: ");
            var email = Console.ReadLine().Trim();

            bool emailExists = (await this._userManager.FindByEmailAsync(email)) != null;
            if (emailExists)
            {
                Log.Warning($"User already exits with the provided email {email}");

                return new AddUserResponse { EmailExists = true };
            }

            user.Email = user.UserName = email;
            user.IsActive = true;

            Console.Write("First Name: ");
            user.Profile.FirstName = Console.ReadLine().Trim();

            Console.Write("Last Name: ");
            user.Profile.LastName = Console.ReadLine().Trim();

            string password = string.Empty;

            if (isDomainUser)
            {
                Console.Write("Domain: ");
                var domain = Console.ReadLine().Trim();

                Console.Write("User Name(Without domain): ");
                var userName = Console.ReadLine().Trim();

                var providerKey = $"{domain}\\{userName}";

                if (await this._userManager.FindByLoginAsync(IdentityConstants.WindowsLoginProvider, providerKey) != null)
                {
                    Log.Warning($"User already exits with the provided domain user {providerKey}");

                    return new AddUserResponse { DomainUserExists = true };
                }

                user.UserLogins.Add(new IdentityUserLogin<string>
                {
                    LoginProvider = IdentityConstants.WindowsLoginProvider,
                    ProviderDisplayName = IdentityConstants.WindowsLoginProviderDisplayName,
                    ProviderKey = providerKey
                });
            }
            else
            {
                // Get password
                var invalidPasswordAttempts = 0;
                do
                {
                    password = await this.Password(user, invalidPasswordAttempts);

                    if (string.IsNullOrWhiteSpace(password))
                    {
                        invalidPasswordAttempts += 1;
                        if (invalidPasswordAttempts < 5)
                        {
                            Console.Write("Invalid password entered, Please try again");
                        }
                        else
                        {
                            Log.Warning($"Invalid password attempts reached for user: {user.Email}");
                        }
                    }
                }
                while (invalidPasswordAttempts < 5 && string.IsNullOrWhiteSpace(password));

                if (invalidPasswordAttempts >= 5 || string.IsNullOrWhiteSpace(password))
                {
                    return new AddUserResponse { PasswordAttemptsExceeded = true };
                }
            }

            var result = await this._userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                Log.Information($"User created successfully with email {user.Email}");

                if (!string.IsNullOrWhiteSpace(password))
                {
                    result = await this._userManager.AddPasswordAsync(user, password);

                    if (result.Succeeded)
                    {
                        Log.Information($"Password added for user {user.Email}");
                    }
                }

                if (isAdmin)
                {
                    try
                    {
                        result = await this._userManager.AddToRolesAsync(user, new string[] { IdentityConstants.SysAdminRole });
                        if (result.Succeeded)
                        {
                            Log.Information($"Added SysAdmin to user {user.Email}");
                            return new AddUserResponse { User = user, AddedUser = true, AddedAdminRole = true };
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"An error occurred while adding sysadmin role to the user {user.Email}\nException: {JsonConvert.SerializeObject(ex)}");
                        return new AddUserResponse { User = user, AddedUser = true, AddedAdminRole = false };
                    }
                }

                return new AddUserResponse { User = user, AddedUser = true };
            }

            Log.Error($"User creation failed for {user.Email}");
            return new AddUserResponse { User = user };
        }

        private async Task<string> Password(ApplicationUser user, int invalidAttempt)
        {
            if (invalidAttempt == 0)
            {
                Console.Write("\nNOTE: Password should contain an uppercase character, lowercase character, a digit, a non-alphanumeric character and must be at least six characters long\n");
            }

            Console.Write("\nPassword: ");

            var password = Console.ReadLine();

            var result = await this._passwordValidator.ValidateAsync(this._userManager, user, password);
            if (!result.Succeeded)
            {
                Log.Warning($"Password attempt {invalidAttempt + 1}\n Error: {JsonConvert.SerializeObject(result.Errors)}");
                return string.Empty;
            }

            return password;
        }

        private bool GetConfirmation()
        {
            ConsoleKey response;

            do
            {
                while (Console.KeyAvailable)
                {
                    Console.ReadKey();
                }

                Console.Write("Type Y for yes, or N for No: ");
                response = Console.ReadKey().Key;
                Console.WriteLine();
            }
            while (response != ConsoleKey.Y && response != ConsoleKey.N);

            return response == ConsoleKey.Y;
        }
    }
}
