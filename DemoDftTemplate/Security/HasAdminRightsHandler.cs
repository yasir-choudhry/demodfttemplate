using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DemoDftTemplate.Security
{
    public class HasAdminRightsHandler : AuthorizationHandler<HasAdminRightsRequirement>
    {
        private readonly ApplicationUser _appUser;

        public HasAdminRightsHandler(ApplicationUser appUser)
        {
            _appUser = appUser;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   HasAdminRightsRequirement requirement)
        {
            if (true) // EXAPMLE _appUser.CurrentUser.user.roles.IsAdmin || _appUser.CurrentUser.user.roles.IsDev
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }

    }
}
