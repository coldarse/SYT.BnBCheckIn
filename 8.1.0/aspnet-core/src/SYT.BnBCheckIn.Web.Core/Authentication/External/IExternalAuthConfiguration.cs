using System.Collections.Generic;

namespace SYT.BnBCheckIn.Authentication.External
{
    public interface IExternalAuthConfiguration
    {
        List<ExternalLoginProviderInfo> Providers { get; }
    }
}
