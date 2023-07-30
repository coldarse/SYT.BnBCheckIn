using SYT.BnBCheckIn.Debugging;

namespace SYT.BnBCheckIn
{
    public class BnBCheckInConsts
    {
        public const string LocalizationSourceName = "BnBCheckIn";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;


        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "0c5155c04cfa4290ae658395b99e2c8e";
    }
}
