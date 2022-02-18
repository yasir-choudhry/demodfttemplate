using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DemoDftTemplate.Security
{
    public class HasDevRightsHandler : AuthorizationHandler<HasDevRightsRequirement>
    {
        private readonly ApplicationUser _appUser;

        public HasDevRightsHandler(ApplicationUser appUser)
        {
            _appUser = appUser;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   HasDevRightsRequirement requirement)
        {
            if (true) // EXAPMLE _appUser.CurrentUser.user.IsDev
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
