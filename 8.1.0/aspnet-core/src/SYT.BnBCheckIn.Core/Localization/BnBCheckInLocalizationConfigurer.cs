using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace SYT.BnBCheckIn.Localization
{
    public static class BnBCheckInLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(BnBCheckInConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(BnBCheckInLocalizationConfigurer).GetAssembly(),
                        "SYT.BnBCheckIn.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
