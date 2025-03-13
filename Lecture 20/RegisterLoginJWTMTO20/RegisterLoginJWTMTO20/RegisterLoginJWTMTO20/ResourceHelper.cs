using RegisterLoginJWTMTO20.Services;
using System.Globalization;
using System.Resources;

namespace RegisterLoginJWTMTO20
{
    public static class ResourceHelper
    {
        public static string GetResource(string resourceName)
        {
            var resourceManager = new ResourceManager("RegisterLoginJWTMTO20.Resources.Resource", typeof(ResourceHelper).Assembly);
            return resourceManager.GetString(resourceName, CultureInfo.CurrentCulture) ?? $"Resource '{resourceName}' not found";
        }
    }
}
