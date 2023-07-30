using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace SYT.BnBCheckIn.EntityFrameworkCore
{
    public static class BnBCheckInDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<BnBCheckInDbContext> builder, string connectionString)
        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
            builder.UseMySql(connectionString, serverVersion);
        }

        public static void Configure(DbContextOptionsBuilder<BnBCheckInDbContext> builder, DbConnection connection)
        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
            builder.UseMySql(connection, serverVersion);
        }
    }
}
