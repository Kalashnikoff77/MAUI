using Data.Dto.Requests;
using Data.Services;

namespace SignalR.Services
{
    public class FormFactor : IFormFactor
    {
        public string GetFormFactor() => "Web";
        public string GetPlatform() => Environment.OSVersion.ToString();

        public async Task StoreLoginDataAsync(LoginRequestDto loginRequestDto)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginRequestDto?> GetLoginDataAsync()
        {
            throw new NotImplementedException();
        }

        public async Task ClearLoginDataAsync()
        {
            throw new NotImplementedException();
        }
    }
}
