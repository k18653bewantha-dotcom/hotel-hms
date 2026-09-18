using HotelBookingSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HotelBookingSystem.Services;

public static class DatabaseSchemaUpgrade
{
    public static void Apply(HotelDbContext context)
    {
        // EnsureCreated does not alter an existing SQLite database.
        // Keep upgrades non-destructive so existing hotel data is preserved.
        context.Database.ExecuteSqlRaw(
            """
            CREATE TABLE IF NOT EXISTS "UserAccounts" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_UserAccounts" PRIMARY KEY AUTOINCREMENT,
                "Username" TEXT NOT NULL,
                "DisplayName" TEXT NOT NULL,
                "PasswordHash" TEXT NOT NULL,
                "Role" TEXT NOT NULL,
                "IsActive" INTEGER NOT NULL,
                "MustChangePassword" INTEGER NOT NULL,
                "CreatedAt" TEXT NOT NULL,
                "LastLoginAt" TEXT NULL
            );
            """);

        context.Database.ExecuteSqlRaw(
            """
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_UserAccounts_Username"
            ON "UserAccounts" ("Username");
            """);

        AddColumnIfMissing(context, "Bookings", "MealPlan",
            "ALTER TABLE \"Bookings\" ADD COLUMN \"MealPlan\" TEXT NOT NULL DEFAULT 'RO';");

        AddColumnIfMissing(context, "Bookings", "Adults",
            "ALTER TABLE \"Bookings\" ADD COLUMN \"Adults\" INTEGER NOT NULL DEFAULT 1;");

        AddColumnIfMissing(context, "Bookings", "Children",
            "ALTER TABLE \"Bookings\" ADD COLUMN \"Children\" INTEGER NOT NULL DEFAULT 0;");

        AddColumnIfMissing(context, "Bookings", "AdvancePayment",
            "ALTER TABLE \"Bookings\" ADD COLUMN \"AdvancePayment\" TEXT NOT NULL DEFAULT '0';");
    }

    private static void AddColumnIfMissing(
        HotelDbContext context,
        string tableName,
        string columnName,
        string alterSql)
    {
        if (ColumnExists(context, tableName, columnName))
        {
            return;
        }

        context.Database.ExecuteSqlRaw(alterSql);
    }

    private static bool ColumnExists(
        HotelDbContext context,
        string tableName,
        string columnName)
    {
        var connection = context.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
        {
            connection.Open();
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info(\"{tableName}\");";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (string.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        finally
        {
            if (shouldClose)
            {
                connection.Close();
            }
        }
    }
}
