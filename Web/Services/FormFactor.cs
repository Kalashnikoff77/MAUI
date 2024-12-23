using Data.Dto.Requests;
using Data.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace SH.Web.Services
{
    public class FormFactor : IFormFactor
    {
        ProtectedLocalStorage _protectedLocalStore { get; set; }
        ProtectedSessionStorage _protectedSessionStore { get; set; }
        
        public FormFactor(ProtectedLocalStorage protectedLocalStore, ProtectedSessionStorage protectedSessionStore)
        {
            _protectedLocalStore = protectedLocalStore;
            _protectedSessionStore = protectedSessionStore;
        }

        public async Task StoreLoginDataAsync(LoginRequestDto loginRequestDto)
        {
            await ClearLoginDataAsync();

            if (loginRequestDto.Remember)
                await _protectedLocalStore.SetAsync(nameof(LoginRequestDto), loginRequestDto);
            else
                await _protectedSessionStore.SetAsync(nameof(LoginRequestDto), loginRequestDto);
        }

        public async Task<LoginRequestDto?> GetLoginDataAsync()
        {
            bool remember = true;
            var storage = await _protectedLocalStore.GetAsync<LoginRequestDto>(nameof(LoginRequestDto));
            if (!storage.Success)
            {
                storage = await _protectedSessionStore.GetAsync<LoginRequestDto>(nameof(LoginRequestDto));
                remember = false;
            }

            if (storage.Success && storage.Value != null)
            {
                storage.Value.Remember = remember;
                return storage.Value;
            }
            else
                return null;
        }

        public async Task ClearLoginDataAsync()
        {
            await _protectedLocalStore.DeleteAsync(nameof(LoginRequestDto));
            await _protectedSessionStore.DeleteAsync(nameof(LoginRequestDto));
        }

        public string GetFormFactor() => "Web";
        public string GetPlatform() => Environment.OSVersion.ToString();
    }
}
