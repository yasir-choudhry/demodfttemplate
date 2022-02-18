using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DemoDftTemplate.Security
{
    public class MySchemeHandler : IAuthenticationHandler
    {
        private HttpContext _context;

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            _context = context;
            return Task.CompletedTask;
        }

        public Task<AuthenticateResult> AuthenticateAsync()
            => Task.FromResult(AuthenticateResult.NoResult());

        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            _context.Response.StatusCode = StatusCodes.Status403Forbidden;
            _context.Response.Redirect("/Home/Disallowed");
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            _context.Response.StatusCode = StatusCodes.Status403Forbidden;
            _context.Response.Redirect("/Home/Disallowed");
            return Task.CompletedTask;
        }

    }
}
