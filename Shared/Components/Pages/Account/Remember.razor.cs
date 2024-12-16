using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net;
using System.Text.RegularExpressions;

namespace Shared.Components.Pages.Account
{
    public partial class Remember
    {
        [Inject] IRepository<RememberRequestDto, ResponseDtoBase> _repoRemember { get; set; } = null!;

        RememberRequestDto rememberRequestDto = new RememberRequestDto();

        string? errorRemember, okRemember = null;

        async void OnRememberAsync()
        {
            errorRemember = null;
            okRemember = null;
            StateHasChanged();

            var apiResponse = await _repoRemember.HttpPostAsync(rememberRequestDto);
            if (apiResponse.StatusCode == HttpStatusCode.OK)
                okRemember = "Пароль отправлен на указанный email!";
            else
                errorRemember = apiResponse.Response.ErrorMessage;

            StateHasChanged();
        }


        Color EmailIconColor = Color.Default;
        string? EmailValidator(string email)
        {
            string? errorMessage = null;
            EmailIconColor = Color.Success;

            if (string.IsNullOrWhiteSpace(email))
            {
                errorMessage = $"Укажите email";
                EmailIconColor = Color.Error;
                return errorMessage;
            }

            if (email.Length < StaticData.DB_ACCOUNTS_EMAIL_MIN)
            {
                errorMessage = $"Email может содержать {StaticData.DB_ACCOUNTS_EMAIL_MIN}-{StaticData.DB_ACCOUNTS_EMAIL_MAX} символов";
                EmailIconColor = Color.Error;
            }

            if (!Regex.IsMatch(email, @"^[a-z0-9_\.-]{1,32}@[a-z0-9\.-]{1,32}\.[a-z]{2,8}$"))
            {
                errorMessage = $"Проверьте корректность email";
                EmailIconColor = Color.Error;
            }

            StateHasChanged();
            return errorMessage;
        }
    }
}
