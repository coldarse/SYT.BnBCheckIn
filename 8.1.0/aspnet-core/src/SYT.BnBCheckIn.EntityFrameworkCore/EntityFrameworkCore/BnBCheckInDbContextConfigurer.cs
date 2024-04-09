using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace SYT.BnBCheckIn.EntityFrameworkCore
{
    public static class BnBCheckInDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<BnBCheckInDbContext> builder, string connectionString)
        {
            var serverVersion = ServerVersion.AutoDetect(connectionString);
            builder.UseMySql(connectionString, serverVersion);
        }

        public static void Configure(DbContextOptionsBuilder<BnBCheckInDbContext> builder, DbConnection connection)
        {
            var serverVersion = ServerVersion.AutoDetect(connection.ConnectionString);
            builder.UseMySql(connection, serverVersion);
        }
    }
}
