using Data.Dto;
using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Extensions;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using Shared.Components.Dialogs;
using Data.State;
using System.Text.RegularExpressions;

namespace Shared.Components.Pages.Account.RegisterAndProfile
{
    public abstract class AccountDtoBase : ComponentBase
    {
        [CascadingParameter] protected CurrentState CurrentState { get; set; } = null!;
        [Inject] protected IRepository<UploadPhotoToTempRequestDto, UploadPhotoToTempResponseDto> _repoUploadPhotoToTemp { get; set; } = null!;
        [Inject] protected IRepository<GetCountriesRequestDto, GetCountriesResponseDto> _repoGetCountries { get; set; } = null!;
        [Inject] protected IRepository<GetHobbiesRequestDto, GetHobbiesResponseDto> _repoGetHobbies { get; set; } = null!;
        [Inject] protected IRepository<LoginRequestDto, LoginResponseDto> _repoLogin { get; set; } = null!;
        [Inject] protected IRepository<GetIdentityRequestDto, GetIdentityResponseDto> _repoGetIdentity { get; set; } = null!;

        [Inject] protected IFormFactor _formFactor { get; set; } = null!;
        [Inject] protected IDialogService _dialogService { get; set; } = null!;
        [Inject] protected IConfiguration _config { get; set; } = null!;
        [Inject] protected IJSProcessor _JSProcessor { get; set; } = null!;

        [Inject] protected IRepository<AccountCheckUpdateRequestDto, AccountCheckUpdateResponseDto> _repoCheckUpdate { get; set; } = null!;
        [Inject] protected IRepository<UpdateAccountRequestDto, ResponseDtoBase> _repoUpdate { get; set; } = null!;
        [Inject] protected IRepository<AccountCheckRegisterRequestDto, AccountCheckRegisterResponseDto> _repoCheckRegister { get; set; } = null!;
        [Inject] protected IRepository<RegisterAccountRequestDto, ResponseDtoBase> _repoRegister { get; set; } = null!;

        /// <summary>
        /// Для предотвращения повторного выполнения OnParametersSet (выполняется при переходе на другую ссылку)
        /// </summary>
        protected bool isFirstSetParameters = true;

        protected Dictionary<short, TabPanel> TabPanels = new Dictionary<short, TabPanel>();

        protected List<CountriesViewDto> Countries { get; set; } = null!;
        protected List<RegionsDto>? Regions { get; set; } = null;

        public AccountRequestDtoBase AccountRequestDto { get; set; } = null!;
        public List<HobbiesDto> Hobbies { get; set; } = null!;

        public bool ProcessingAccount, ProcessingPhoto, IsDataSaved = false;

        public bool IsPanel1Valid => TabPanels[1].Items.All(x => x.Value == true);
        public bool IsPanel2Valid => TabPanels[2].Items.All(x => x.Value == true);
        public bool IsPanel3Valid => TabPanels[3].Items.All(x => x.Value == true);
        public bool IsPanel4Valid => TabPanels[4].Items.All(x => x.Value == true);


        #region /// ШАГ 1: ОБЩЕЕ ///
        string? _countryText;
        public string? CountryText
        {
            get => _countryText;
            set
            {
                if (value != null && Countries.Any())
                {
                    var country = Countries.Where(c => c.Name == value).FirstOrDefault();
                    if (country != null)
                    {
                        AccountRequestDto.Country.Id = country.Id;
                        Regions = country.Regions;
                    }
                }
                _countryText = value;
                RegionText = null;
            }
        }

        string? _regionText;
        public string? RegionText
        {
            get => _regionText;
            set
            {
                if (value != null && Countries.Any() && Regions != null)
                {
                    var region = Regions.Where(c => c.Name == value).FirstOrDefault();
                    if (region != null)
                        AccountRequestDto.Country.Region.Id = region.Id;
                }
                _regionText = value;
            }
        }

        public Color NameIconColor = Color.Default;
        public async Task<string?> NameValidator(string name)
        {
            string? errorMessage = null;
            if (string.IsNullOrWhiteSpace(name) || name.Length < StaticData.DB_ACCOUNTS_NAME_MIN)
                errorMessage = $"Имя должно содержать {StaticData.DB_ACCOUNTS_NAME_MIN}-{StaticData.DB_ACCOUNTS_NAME_MAX} символов";

            // Если регистрация
            if (AccountRequestDto is RegisterAccountRequestDto)
            {
                var apiResponse = await _repoCheckRegister.HttpPostAsync(new AccountCheckRegisterRequestDto { AccountName = name });
                if (apiResponse.Response.AccountNameExists)
                    errorMessage = $"Это имя уже занято. Выберите другое.";
            }
            // Если обновление
            else
            {
                var apiResponse = await _repoCheckUpdate.HttpPostAsync(new AccountCheckUpdateRequestDto { AccountName = name, Token = CurrentState.Account?.Token });
                if (apiResponse.Response.AccountNameExists)
                    errorMessage = $"Это имя уже занято. Выберите другое.";
            }

            CheckPanel1Properties(errorMessage, nameof(AccountRequestDto.Name), ref NameIconColor);

            return errorMessage;
        }

        public Color EmailIconColor = Color.Default;
        public async Task<string?> EmailValidator(string email)
        {
            string? errorMessage = null;

            if (string.IsNullOrWhiteSpace(email) || email.Length < StaticData.DB_ACCOUNTS_EMAIL_MIN)
                errorMessage = $"Email может содержать {StaticData.DB_ACCOUNTS_EMAIL_MIN}-{StaticData.DB_ACCOUNTS_EMAIL_MAX} символов";

            if (!Regex.IsMatch(email, @"^[a-z0-9_\.-]{1,32}@[a-z0-9\.-]{1,32}\.[a-z]{2,8}$"))
                errorMessage = $"Проверьте корректность email";

            // Если регистрация
            if (AccountRequestDto is RegisterAccountRequestDto)
            {
                var apiResponse = await _repoCheckRegister.HttpPostAsync(new AccountCheckRegisterRequestDto { AccountEmail = email });
                if (apiResponse.Response.AccountEmailExists)
                    errorMessage = $"Этот email уже зарегистрирован. Забыли пароль?";
            }
            // Если обновление
            else
            {
                var apiResponse = await _repoCheckUpdate.HttpPostAsync(new AccountCheckUpdateRequestDto { AccountEmail = email, Token = CurrentState.Account?.Token });
                if (apiResponse.Response.AccountEmailExists)
                    errorMessage = $"Этот email уже занят другим пользователем.";
            }

            CheckPanel1Properties(errorMessage, nameof(AccountRequestDto.Email), ref EmailIconColor);

            return errorMessage;
        }

        public Color PasswordIconColor = Color.Default;
        public string? Password
        {
            get => AccountRequestDto.Password;
            set
            {
                AccountRequestDto.Password = value!;
                Password2 = null;
            }
        }
        public string? PasswordValidator(string password)
        {
            string? errorMessage = null;
            if (string.IsNullOrWhiteSpace(password) || password.Length < StaticData.DB_ACCOUNTS_PASSWORD_MIN)
                errorMessage = $"Пароль должен содержать {StaticData.DB_ACCOUNTS_PASSWORD_MIN}-{StaticData.DB_ACCOUNTS_PASSWORD_MAX} символов";

            CheckPanel1Properties(errorMessage, nameof(AccountRequestDto.Password), ref PasswordIconColor);
            return errorMessage;
        }

        public Color Password2IconColor = Color.Default;
        public string? Password2
        {
            get => AccountRequestDto.Password2;
            set => AccountRequestDto.Password2 = value!;
        }
        public string? Password2Validator(string password2)
        {
            string? errorMessage = null;
            if (AccountRequestDto.Password != password2)
                errorMessage = $"Пароли не совпадают";

            CheckPanel1Properties(errorMessage, nameof(AccountRequestDto.Password2), ref Password2IconColor);
            return errorMessage;
        }


        public Color CountryIconColor = Color.Default;
        public string? CountryValidator(string country)
        {
            string? errorMessage = null;
            if (string.IsNullOrWhiteSpace(CountryText))
                errorMessage = $"Выберите страну";

            // Сбросим в false регион
            TabPanels[1].Items[nameof(AccountRequestDto.Country.Region)] = false;
            RegionText = null;
            RegionIconColor = Color.Default;

            CheckPanel1Properties(errorMessage, nameof(AccountRequestDto.Country), ref CountryIconColor);
            return errorMessage;
        }

        public Color RegionIconColor = Color.Default;
        public string? RegionValidator(string region)
        {
            string? errorMessage = null;
            if (string.IsNullOrWhiteSpace(RegionText))
                errorMessage = $"Выберите регион";

            CheckPanel1Properties(errorMessage, nameof(AccountRequestDto.Country.Region), ref RegionIconColor);
            return errorMessage;
        }

        void CheckPanel1Properties(string? errorMessage, string property, ref Color iconColor)
        {
            if (errorMessage == null)
            {
                TabPanels[1].Items[property] = true;
                iconColor = Color.Success;
            }
            else
            {
                TabPanels[1].Items[property] = false;
                iconColor = Color.Error;
            }
            StateHasChanged();
        }


        public async Task<IEnumerable<string>?> SearchCountryAsync(string value, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Countries.Select(s => s.Name);

            return Countries?.Select(s => s.Name)
                .Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<IEnumerable<string>?> SearchRegionAsync(string value, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Regions?.Select(s => s.Name);

            return Regions?.Select(s => s.Name)
                .Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }
        #endregion


        #region /// ШАГ 2: ПАРТНЁРЫ ///
        public async Task DeleteUserDialogAsync(UsersDto user)
        {
            var parameters = new DialogParameters<ConfirmDialog>
            {
                { x => x.ContentText, $"Удалить партнёра {user.Name}?" },
                { x => x.ButtonText, "Удалить" },
                { x => x.Color, Color.Error }
            };
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };
            var resultDialog = await _dialogService.ShowAsync<ConfirmDialog>($"Удаление {user.Name}", parameters, options);
            var result = await resultDialog.Result;

            if (result != null && result.Canceled == false && AccountRequestDto.Users.Contains(user))
                AccountRequestDto.Users.Remove(user);

            CheckPanel2Properties();
        }

        public async Task AddUserDialogAsync(MouseEventArgs args)
        {
            var parameters = new DialogParameters<EditUserDialog> { { x => x.User, null } };
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };

            var resultDialog = await _dialogService.ShowAsync<EditUserDialog>("Добавление партнёра", parameters, options);
            var result = await resultDialog.Result;
            if (result != null && result.Canceled == false && result.Data != null)
                AccountRequestDto.Users.Add((UsersDto)result.Data);

            CheckPanel2Properties();
        }

        public async Task UpdateUserDialogAsync(UsersDto user)
        {
            var parameters = new DialogParameters<EditUserDialog> { { x => x.User, user } };
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };

            var resultDialog = await _dialogService.ShowAsync<EditUserDialog>("Редактирование партнёра", parameters, options);
            var result = await resultDialog.Result;
            if (result != null && result.Canceled == false && result.Data != null)
            {
                var position = AccountRequestDto.Users.IndexOf(user);
                AccountRequestDto.Users.RemoveAt(position);
                AccountRequestDto.Users.Insert(position, result.Data.DeepCopy<UsersDto>()!);
            }
        }

        void CheckPanel2Properties()
        {
            TabPanels[2].Items[nameof(AccountRequestDto.Users)] = AccountRequestDto.Users.Count == 0 ? false : true;
            StateHasChanged();
        }
        #endregion


        #region /// ШАГ 3: ХОББИ ///
        public void OnHobbyChanged(HobbiesDto hobby)
        {
            if (AccountRequestDto.Hobbies != null)
            {
                var index = AccountRequestDto.Hobbies.FindIndex(x => x.Id == hobby.Id);
                if (index >= 0)
                    AccountRequestDto.Hobbies.RemoveAt(index);
                else
                    AccountRequestDto.Hobbies.Add(hobby);
            }
            else
                AccountRequestDto.Hobbies = [hobby];

            CheckPanel3Properties();
        }

        void CheckPanel3Properties()
        {
            if (AccountRequestDto.Hobbies != null && AccountRequestDto.Hobbies.Any())
                TabPanels[3].Items[nameof(AccountRequestDto.Hobbies)] = true;
            else
                TabPanels[3].Items[nameof(AccountRequestDto.Hobbies)] = false;
            StateHasChanged();
        }
        #endregion


        #region /// ШАГ 4: ФОТО ///
        public async void UploadPhotosAsync(IReadOnlyList<IBrowserFile> browserPhotos)
        {
            if (browserPhotos.Count > 0)
            {
                ProcessingPhoto = true;
                StateHasChanged();

                if (AccountRequestDto.Photos == null)
                    AccountRequestDto.Photos = new List<PhotosForAccountsDto>();

                foreach (var photo in browserPhotos)
                {
                    var newPhoto = await photo.Upload<PhotosForAccountsDto>(CurrentState.Account?.Token, _repoUploadPhotoToTemp, accountId: CurrentState.Account?.Id ?? 0);

                    if (newPhoto != null)
                    {
                        // Если это первая фотка, то отметим её как аватар
                        if (AccountRequestDto.Photos.Count(x => x.IsDeleted == false) == 0)
                            newPhoto.IsAvatar = true;
                        AccountRequestDto.Photos.Insert(0, newPhoto);
                    }
                    if (AccountRequestDto.Photos.Count(x => x.IsDeleted == false) >= 20) break;
                }

                ProcessingPhoto = false;

                CheckPanel4Properties();
            }
        }

        public void UpdateCommentPhoto(PhotosForAccountsDto photo, string comment) =>
            photo.Comment = comment;

        public void SetAsAvatarPhoto(PhotosForAccountsDto photo)
        {
            AccountRequestDto.Photos?.ForEach(x => x.IsAvatar = false);
            photo.IsAvatar = true;
        }

        void CheckPanel4Properties()
        {
            if (AccountRequestDto.Photos.Count(x => x.IsDeleted == false) > 0)
                TabPanels[4].Items["Photos"] = true;
            else
                TabPanels[4].Items["Photos"] = false;

            StateHasChanged();
        }
        #endregion


        public void Dispose()
        {
            if (AccountRequestDto?.Photos != null)
                foreach (var photo in AccountRequestDto.Photos.Where(w => w.Id == 0))
                    if (Directory.Exists(StaticData.TempPhotosDir + "/" + photo.Guid))
                        Directory.Delete(StaticData.TempPhotosDir + "/" + photo.Guid, true);
        }
    }
}
