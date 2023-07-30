using Abp.Authorization;
using SYT.BnBCheckIn.Authorization.Roles;
using SYT.BnBCheckIn.Authorization.Users;

namespace SYT.BnBCheckIn.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
