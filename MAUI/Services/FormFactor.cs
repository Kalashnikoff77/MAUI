using Shared.Services;

namespace MAUI.Services
{
    public class FormFactor : IFormFactor
    {
        public string GetFormFactor()
        {
            return DeviceInfo.Idiom.ToString();
        }

        public string GetPlatform()
        {
            return DeviceInfo.Platform.ToString() + " - " + DeviceInfo.VersionString;
        }
    }
}