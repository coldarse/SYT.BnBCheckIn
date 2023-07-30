using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using SYT.BnBCheckIn.Authorization.Roles;
using SYT.BnBCheckIn.Authorization.Users;
using SYT.BnBCheckIn.MultiTenancy;
/* Using Definition */
using SYT.BnBCheckIn.Usages;
using SYT.BnBCheckIn.Units;
using SYT.BnBCheckIn.Buildings;
using SYT.BnBCheckIn.RFIDS;
using SYT.BnBCheckIn.Picos;

namespace SYT.BnBCheckIn.EntityFrameworkCore
{
    public class BnBCheckInDbContext : AbpZeroDbContext<Tenant, Role, User, BnBCheckInDbContext>
    {
        public DbSet<Pico> Picos { get; set; }
        public DbSet<RFID> RFIDS { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Usage> Usages { get; set; }
        /* Define a DbSet for each entity of the application */
        
        public BnBCheckInDbContext(DbContextOptions<BnBCheckInDbContext> options)
            : base(options)
        {
        }
    }
}
