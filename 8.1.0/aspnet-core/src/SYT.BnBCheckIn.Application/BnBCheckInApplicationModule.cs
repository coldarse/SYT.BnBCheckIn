using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using SYT.BnBCheckIn.Authorization;

namespace SYT.BnBCheckIn
{
    [DependsOn(
        typeof(BnBCheckInCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class BnBCheckInApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<BnBCheckInAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(BnBCheckInApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
