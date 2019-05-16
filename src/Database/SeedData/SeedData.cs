using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;
using System.Reflection;

namespace Database.SeedData
{
    public static class SeedData
    {
        public static void InsertMasterData(this MigrationBuilder migrationBuilder)
        {
            string[] files = Directory.GetFiles(
                Assembly.GetExecutingAssembly().Location.Replace("Database.dll", "Data",
                    StringComparison.CurrentCultureIgnoreCase), "*.sql");
            foreach (string file in files)
            {
                migrationBuilder.Sql(File.ReadAllText(file));
            }
        }
    }
}