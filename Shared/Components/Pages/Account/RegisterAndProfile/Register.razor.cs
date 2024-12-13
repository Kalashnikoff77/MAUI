using Common.Dto;
using Common.Dto.Requests;
using Common.Models;
using System.Net;

namespace Shared.Components.Pages.Account.RegisterAndProfile
{
    public partial class Register : AccountDtoBase, IDisposable
    {
        protected override async Task OnInitializedAsync()
        {
            TabPanels = new Dictionary<short, TabPanel>()
            {
                { 1, new TabPanel
                    {
                        Items = new Dictionary<string, bool>
                        {
                            { nameof(AccountRequestDto.Name), false },
                            { nameof(AccountRequestDto.Email), false },
                            { nameof(AccountRequestDto.Password), false },
                            { nameof(AccountRequestDto.Password2), false },
                            { nameof(AccountRequestDto.Country), false },
                            { nameof(AccountRequestDto.Country.Region), false }
                        }
                    }
                },
                { 2, new TabPanel { Items = new Dictionary<string, bool> { { nameof(AccountRequestDto.Users), false } } } },
                { 3, new TabPanel { Items = new Dictionary<string, bool> { { nameof(AccountRequestDto.Hobbies), false } } } },
                { 4, new TabPanel { Items = new Dictionary<string, bool> { { "Photos", false } } } }
            };

            var apiCountriesResponse = await _repoGetCountries.HttpPostAsync(new GetCountriesRequestDto());
            Countries = apiCountriesResponse.Response.Countries;

            var apiHobbiesResponse = await _repoGetHobbies.HttpPostAsync(new GetHobbiesRequestDto());
            Hobbies = apiHobbiesResponse.Response.Hobbies;

            // TODO УДАЛИТЬ ЗНАЧЕНИЯ ПО УМОЛЧАНИЮ (OK)
            AccountRequestDto = new RegisterAccountRequestDto
            {
                Name = "Олег и Марина Мск2",
                Email = "olegmar@mail.ru",
                Password = "pass1234",
                Password2 = "pass1234",
                Users = new List<UsersDto>
                {
                    new UsersDto { Id = 0, Name = "Олег", Gender = 0, Weight=80, Height=180, BirthDate = DateTime.Parse("29.01.1977") },
                    new UsersDto { Id = 1, Name = "Марина", Gender = 1, Weight=74, Height=173, BirthDate = DateTime.Parse("01.07.1969") }
                }
            };
        }


        async void SubmitAsync()
        {
            AccountRequestDto.ErrorMessage = null;
            ProcessingAccount = true;
            StateHasChanged();

            var response = await _repoRegister.HttpPostAsync((RegisterAccountRequestDto)AccountRequestDto);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                AccountRequestDto.ErrorMessage = response.Response.ErrorMessage;
                ProcessingAccount = false;
                StateHasChanged();
            }
            else
            {
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

                    //if (loginRequestDto.Remember)
                    //    await _protectedLocalStore.SetAsync(nameof(LoginRequestDto), loginRequestDto);
                    //else
                    //    await _protectedSessionStore.SetAsync(nameof(LoginRequestDto), loginRequestDto);

                    await _JSProcessor.Redirect("/");
                }
                else
                {
                    AccountRequestDto.ErrorMessage = apiResponse.Response.ErrorMessage;
                    StateHasChanged();
                }
            }
        }
    }
}
