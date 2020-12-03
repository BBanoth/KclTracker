// <copyright file="Program.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Database.Sql
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.Extensions.Configuration;

    public static class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .Build();

        public static void Main(string[] args)
        {
            var connectionString = Configuration.GetConnectionString("KclTracker");

            if (args.Length > 0 && args.ElementAt(0) == ArgumentActions.CleanDatabase)
            {
                Console.WriteLine(@"Deleting the database");
                DatabaseMigration.DropDatabase(connectionString);
            }

            Console.WriteLine(@"Database migration started");
            var result = DatabaseMigration.Upgrade(connectionString);

            if (result.Successful)
            {
                Console.WriteLine(@"Database migrated successfully");
            }
            else
            {
                Console.WriteLine(@"Database migration failed");

                // exit for the octopus to set the deploy fail
                Environment.Exit(-1);
            }
        }
    }
}
