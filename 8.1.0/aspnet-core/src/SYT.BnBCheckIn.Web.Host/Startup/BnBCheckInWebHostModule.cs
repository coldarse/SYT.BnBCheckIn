using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using SYT.BnBCheckIn.Configuration;

namespace SYT.BnBCheckIn.Web.Host.Startup
{
    [DependsOn(
       typeof(BnBCheckInWebCoreModule))]
    public class BnBCheckInWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public BnBCheckInWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BnBCheckInWebHostModule).GetAssembly());
        }
    }
}
