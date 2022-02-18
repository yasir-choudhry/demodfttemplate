using Microsoft.AspNetCore.Http;

namespace DemoDftTemplate.Helpers
{
    public static class WebHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static HttpContext HttpContext
        {
            get { return _httpContextAccessor.HttpContext; }
        }

        public static string ipAddress
        {
            get { return _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(); }
        }
    }
}
