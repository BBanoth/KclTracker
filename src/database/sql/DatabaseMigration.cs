// <copyright file="DatabaseMigration.cs" company="Agility E Services">
// Copyright (c) Agility E Services. All rights reserved.
// </copyright>

namespace KclTracker.Database.Sql
{
    using System;
    using System.Reflection;
    using DbUp;
    using DbUp.Engine;

    public static class DatabaseMigration
    {
        public static void DropDatabase(string connectionString)
        {
            // Drop Database if exists
            DbUp.DropDatabase.For.SqlDatabase(connectionString);
        }

        public static DatabaseUpgradeResult Upgrade(string connectionString)
        {
            // Create Database if not exists
            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .WithExecutionTimeout(TimeSpan.FromMinutes(3))
                .Build();

            return upgrader.PerformUpgrade();
        }
    }
}
