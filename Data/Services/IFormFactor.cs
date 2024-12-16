using Data.Dto.Requests;

namespace Data.Services
{
    public interface IFormFactor
    {
        public string GetFormFactor();
        public string GetPlatform();

        /// <summary>
        /// ���������� ������ � ������ (����� ����� � �������) � �������� ��� � ������
        /// </summary>
        public Task StoreLoginDataAsync(LoginRequestDto loginRequestDto);

        /// <summary>
        /// ��������� ������ � ������
        /// </summary>
        public Task<LoginRequestDto?> GetLoginDataAsync();

        /// <summary>
        /// ������� ��������� �������� ��� ������
        /// </summary>
        public Task ClearLoginDataAsync();
    }
}
