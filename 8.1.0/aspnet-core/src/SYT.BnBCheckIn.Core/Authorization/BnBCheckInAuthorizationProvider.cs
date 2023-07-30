using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace SYT.BnBCheckIn.Authorization
{
    public class BnBCheckInAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);

            /* Define your permissions here */
            context.CreatePermission(PermissionNames.Pages_Usage, L("Usages"));
            context.CreatePermission(PermissionNames.Pages_Usage_Create, L("UsagesCreate"));
            context.CreatePermission(PermissionNames.Pages_Usage_Edit, L("UsagesEdit"));
            context.CreatePermission(PermissionNames.Pages_Usage_Delete, L("UsagesDelete"));

            context.CreatePermission(PermissionNames.Pages_Unit, L("Units"));
            context.CreatePermission(PermissionNames.Pages_Unit_Create, L("UnitsCreate"));
            context.CreatePermission(PermissionNames.Pages_Unit_Edit, L("UnitsEdit"));
            context.CreatePermission(PermissionNames.Pages_Unit_Delete, L("UnitsDelete"));

            context.CreatePermission(PermissionNames.Pages_Building, L("Buildings"));
            context.CreatePermission(PermissionNames.Pages_Building_Create, L("BuildingsCreate"));
            context.CreatePermission(PermissionNames.Pages_Building_Edit, L("BuildingsEdit"));
            context.CreatePermission(PermissionNames.Pages_Building_Delete, L("BuildingsDelete"));

            context.CreatePermission(PermissionNames.Pages_RFID, L("RFIDS"));
            context.CreatePermission(PermissionNames.Pages_RFID_Create, L("RFIDSCreate"));
            context.CreatePermission(PermissionNames.Pages_RFID_Edit, L("RFIDSEdit"));
            context.CreatePermission(PermissionNames.Pages_RFID_Delete, L("RFIDSDelete"));

            context.CreatePermission(PermissionNames.Pages_Pico, L("Picos"));
            context.CreatePermission(PermissionNames.Pages_Pico_Create, L("PicosCreate"));
            context.CreatePermission(PermissionNames.Pages_Pico_Edit, L("PicosEdit"));
            context.CreatePermission(PermissionNames.Pages_Pico_Delete, L("PicosDelete"));


        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, BnBCheckInConsts.LocalizationSourceName);
        }
    }
}
