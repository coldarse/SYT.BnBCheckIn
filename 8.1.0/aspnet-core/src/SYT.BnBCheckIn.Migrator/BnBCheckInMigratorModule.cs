using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using SYT.BnBCheckIn.Configuration;
using SYT.BnBCheckIn.EntityFrameworkCore;
using SYT.BnBCheckIn.Migrator.DependencyInjection;

namespace SYT.BnBCheckIn.Migrator
{
    [DependsOn(typeof(BnBCheckInEntityFrameworkModule))]
    public class BnBCheckInMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public BnBCheckInMigratorModule(BnBCheckInEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(BnBCheckInMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                BnBCheckInConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BnBCheckInMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
