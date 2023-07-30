using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using SYT.BnBCheckIn.EntityFrameworkCore;
using SYT.BnBCheckIn.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace SYT.BnBCheckIn.Web.Tests
{
    [DependsOn(
        typeof(BnBCheckInWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class BnBCheckInWebTestModule : AbpModule
    {
        public BnBCheckInWebTestModule(BnBCheckInEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BnBCheckInWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(BnBCheckInWebMvcModule).Assembly);
        }
    }
}