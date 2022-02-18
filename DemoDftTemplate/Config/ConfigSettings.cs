
namespace DemoDftTemplate.Config
{
    public class ConfigSettings
    {
        public string IapClientId { get; set; }
        public string APIUrl { get; set; }
        public string Environment { get; set; }
    }

    public enum ApiMethods
    {
        GET,
        POST,
        PUT,
        DELETE
    }
}
