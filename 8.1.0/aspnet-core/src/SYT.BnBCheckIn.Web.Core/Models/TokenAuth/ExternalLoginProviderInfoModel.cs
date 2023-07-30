using Abp.AutoMapper;
using SYT.BnBCheckIn.Authentication.External;

namespace SYT.BnBCheckIn.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }
    }
}
