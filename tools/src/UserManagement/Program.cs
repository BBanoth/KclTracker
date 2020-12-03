// <copyright file="Program.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Tools.UserManagement
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using KclTracker.Tools.UserManagement.Constants;
    using KclTracker.Tools.UserManagement.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    public class Program
    {
        private static IServiceProvider _provider;

        private static UserService UserService
        {
            get
            {
                return new UserService(_provider);
            }
        }

        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        private static IConfiguration SeriLogConfiguration { get; } = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("serilogconfig.json", optional: false, reloadOnChange: true)
           .Build();

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .ReadFrom.Configuration(SeriLogConfiguration)
               .Enrich.FromLogContext()
               .CreateLogger();

            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            _provider = serviceCollection.BuildServiceProvider();

            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    if (arg == ArgumentActions.SeedUsersAndRoles)
                    {
                        try
                        {
                            var result = await UserService.CreateSysAdminRoleAsync();
                            if (result.Succeeded)
                            {
                                Console.WriteLine("Added SysAdmin role");

                                var userSection = Configuration.GetSection("DefaultUser");

                                var user = new ApplicationUser
                                {
                                    Email = userSection["Email"],
                                    UserName = userSection["Email"],
                                    UserTypeId = UserType.Admin,
                                    IsActive = true,
                                    Profile = new UserProfile
                                    {
                                        FirstName = userSection["FirstName"],
                                        LastName = userSection["LastName"],
                                    },
                                };

                                result = await UserService.AddUserAsync(user, userSection["Password"]);
                                if (result.Succeeded)
                                {
                                    Console.WriteLine("Added default user from configuration file");

                                    result = await UserService.AssignSysAdminRoleAsync(user.Email);
                                    if (result.Succeeded)
                                    {
                                        Console.WriteLine("Assigned SysAdmin role to the default user");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        await UserService.ImportDomainUsersFromFileAsync();

                        Console.WriteLine("Imported domain users from file");
                    }
                }
            }
            else
            {
                do
                {
                    Console.WriteLine("Please select the desired option from below list");
                    Console.WriteLine("1. Add SysAdmin role to the system\n2. Add an admin user\n3. Add an user\n4. Add domain admin user\n5. Add domain user\n6. Import domain users from file\n7. Exit");
                    Console.Write("Please enter the option: ");

                    if (!Enum.TryParse(Console.ReadLine(), out WorkOption option))
                    {
                        Console.Write("\n************************************************************\n");
                        Console.Write("Please select a valid option to continue");
                        Console.Write("\n************************************************************\n\n");
                    }
                    else
                    {
                        switch (option)
                        {
                            case WorkOption.CreateSysAdminRole:
                                await UserService.AddSysAdminRoleAsync();
                                break;
                            case WorkOption.AddAdminUser:
                                await UserService.AddUserAsync(option);
                                break;
                            case WorkOption.AddUser:
                                await UserService.AddUserAsync(option);
                                break;
                            case WorkOption.AddDomainAdminUser:
                                await UserService.AddUserAsync(option);
                                break;
                            case WorkOption.AddDomainUser:
                                await UserService.AddUserAsync(option);
                                break;
                            case WorkOption.ImportDomainUsersFromFile:
                                await UserService.ImportDomainUsersFromFileAsync();
                                break;
                            default:
                                break;
                        }
                    }
                }
                while (true);
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Logging
            services.AddLogging(config =>
            {
                config.AddSerilog();
            });

            // Database Context Configuration
            var connectionString = Configuration.GetConnectionString("KclTracker");
            services.AddDbContext<AddUserDbContext>(options => options.UseSqlServer(connectionString));

            // Identity Configuration
            services.AddIdentityCore<ApplicationUser>()
               .AddRoles<ApplicationRole>()
               .AddEntityFrameworkStores<AddUserDbContext>();
        }
    }
}
