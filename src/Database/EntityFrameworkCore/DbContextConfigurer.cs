using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Database.EntityFrameworkCore
{
    public static class DbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<APIDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<APIDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}