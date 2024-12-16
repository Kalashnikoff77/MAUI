using Data.Dto.Requests;

namespace Data.Services
{
    public interface IFormFactor
    {
        public string GetFormFactor();
        public string GetPlatform();

        /// <summary>
        /// Сохранение логина и пароля (после входа в систему) в браузере или в мобиле
        /// </summary>
        public Task StoreLoginDataAsync(LoginRequestDto loginRequestDto);

        /// <summary>
        /// Получение логина и пароля
        /// </summary>
        public Task<LoginRequestDto?> GetLoginDataAsync();

        /// <summary>
        /// Очистка хранилища браузера или мобилы
        /// </summary>
        public Task ClearLoginDataAsync();
    }
}
