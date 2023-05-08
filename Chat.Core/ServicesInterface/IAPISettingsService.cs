using Chat.Communication.ViewObjects.APISettings;

namespace Chat.Core.ServicesInterface
{
    public interface IAPISettingsService
    {
        public AppSettingsVO GetInfoAppSettings();
        public CurrentRequestVO GetInfoCurrentRequest(bool isAuthenticatedRequest = false);
    }
}
