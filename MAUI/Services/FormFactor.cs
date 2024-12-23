using Data.Dto.Requests;
using Data.Services;

namespace MAUI.Services
{
    public class FormFactor : IFormFactor
    {
        public Task StoreLoginDataAsync(LoginRequestDto loginRequestDto)
        {
            Preferences.Set(nameof(loginRequestDto.Email), loginRequestDto.Email);
            Preferences.Set(nameof(loginRequestDto.Password), loginRequestDto.Password);
            Preferences.Set(nameof(loginRequestDto.Remember), loginRequestDto.Remember);
            return Task.CompletedTask;
        }

        public async Task<LoginRequestDto?> GetLoginDataAsync()
        {
            LoginRequestDto loginRequestDto = new LoginRequestDto
            {
                Email = Preferences.Get(nameof(loginRequestDto.Email), null),
                Password = Preferences.Get(nameof(loginRequestDto.Password), null),
                Remember = Preferences.Get(nameof(loginRequestDto.Remember), false),
            };
            return loginRequestDto.Email == null ? null : loginRequestDto;
        }

        public Task ClearLoginDataAsync()
        {
            Preferences.Clear();
            return Task.CompletedTask;
        }


        public string GetFormFactor() => DeviceInfo.Idiom.ToString();
        public string GetPlatform() => DeviceInfo.Platform.ToString() + " - " + DeviceInfo.VersionString;
    }
}
