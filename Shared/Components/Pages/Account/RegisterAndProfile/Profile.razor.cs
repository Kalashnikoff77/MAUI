using Data.Dto.Requests;
using Data.Extensions;
using Data.Models;
using Data.Models.SignalR;
using Data.State;
using MudBlazor;
using System.Net;

namespace Shared.Components.Pages.Account.RegisterAndProfile
{
    public partial class Profile : AccountDtoBase, IDisposable
    {
        protected override async Task OnInitializedAsync()
        {
            TabPanels = new Dictionary<short, TabPanel>()
            {
                { 1, new TabPanel
                    {
                        Items = new Dictionary<string, bool>
                        {
                            { nameof(AccountRequestDto.Name), true },
                            { nameof(AccountRequestDto.Email), true },
                            { nameof(AccountRequestDto.Password), true },
                            { nameof(AccountRequestDto.Password2), true },
                            { nameof(AccountRequestDto.Country), true },
                            { nameof(AccountRequestDto.Country.Region), true }
                        }
                    }
                },
                { 2, new TabPanel { Items = new Dictionary<string, bool> { { nameof(AccountRequestDto.Users), true } } } },
                { 3, new TabPanel { Items = new Dictionary<string, bool> { { nameof(AccountRequestDto.Hobbies), true } } } },
                { 4, new TabPanel { Items = new Dictionary<string, bool> { { "Photos", true } } } }
            };

            var apiCountriesResponse = await _repoGetCountries.HttpPostAsync(new GetCountriesRequestDto());
            Countries = apiCountriesResponse.Response.Countries;

            var apiHobbiesResponse = await _repoGetHobbies.HttpPostAsync(new GetHobbiesRequestDto());
            Hobbies = apiHobbiesResponse.Response.Hobbies;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (CurrentState.Account != null && isFirstSetParameters)
            {
                var apiIdentityResponse = await _repoGetIdentity.HttpPostAsync(new GetIdentityRequestDto() { Token = CurrentState.Account.Token });

                AccountRequestDto = CurrentState.Account.DeepCopy<UpdateAccountRequestDto>()!;
                AccountRequestDto.Email = apiIdentityResponse.Response.Identity.Email;
                AccountRequestDto.Password = apiIdentityResponse.Response.Identity.Password;
                AccountRequestDto.Password2 = apiIdentityResponse.Response.Identity.Password;

                CountryText = CurrentState.Account.Country!.Name;
                RegionText = CurrentState.Account.Country!.Region.Name;

                var loginData = await _formFactor.GetLoginDataAsync();
                AccountRequestDto.Remember = loginData!.Remember;

                NameIconColor = EmailIconColor = PasswordIconColor = Password2IconColor = CountryIconColor = RegionIconColor = Color.Success;

                isFirstSetParameters = false;
            }
        }


        async void SubmitAsync()
        {
            AccountRequestDto.ErrorMessage = null;
            ProcessingAccount = true;
            StateHasChanged();

            var response = await _repoUpdate.HttpPostAsync((UpdateAccountRequestDto)AccountRequestDto);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                AccountRequestDto.ErrorMessage = response.Response.ErrorMessage;
            }
            else
            {
                var newAvatar = AccountRequestDto.Photos.First(x => x.IsAvatar == true);
                if (CurrentState.Account?.Avatar!.Id != newAvatar.Id)
                {
                    var signalRequest = new SignalGlobalRequest
                    {
                        OnAvatarChangedRequest = new OnAvatarChangedRequest { NewAvatar = newAvatar }
                    };
                    await CurrentState.SignalRServerAsync(signalRequest);
                }

                LoginRequestDto loginRequestDto = new LoginRequestDto
                {
                    Email = AccountRequestDto.Email,
                    Password = AccountRequestDto.Password,
                    Remember = AccountRequestDto.Remember
                };

                var apiResponse = await _repoLogin.HttpPostAsync(loginRequestDto);

                if (apiResponse.StatusCode == HttpStatusCode.OK)
                {
                    apiResponse.Response.Account!.Token = StaticData.GenerateToken(apiResponse.Response.Account.Id, apiResponse.Response.Account.Guid, _config);
                    CurrentState.SetAccount(apiResponse.Response.Account);

                    await _formFactor.StoreLoginDataAsync(loginRequestDto);

                    IsDataSaved = true;
                }
                else
                {
                    AccountRequestDto.ErrorMessage = apiResponse.Response.ErrorMessage;
                }
            }

            ProcessingAccount = false;
            CurrentState.StateHasChanged();
        }
    }
}
