using DemoDftTemplate.Config;
using DemoDftTemplate.Helpers;
using Microsoft.Extensions.Options;

namespace DemoDftTemplate
{
    public class ApplicationUser
    {
        public string Email { get; set; }

        public ApplicationUser(IOptions<ConfigSettings> config)
        {
            ConfigSettings _config = config.Value;

            //get user or from GCP header if using on app engine
            var currentUserEmail = Utility.GetCurrentUserEmail();

            //write code to check CurrentUser is in the application database

        }
    }

}