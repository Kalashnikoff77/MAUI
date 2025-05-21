using Data.Dto;
using Data.Extensions;
using Data.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Shared.Components.Pages.Account.RegisterAndProfile
{
    public partial class EditUserDialog
    {
        [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = null!;
        [Parameter] public UsersDto User { get; set; } = null!;

        Dictionary<short, DialogProperty> dialogProperties { get; set; } = null!;
        bool isFormValid => dialogProperties[1].Items.All(x => x.Value == true);

        UsersDto UserCopy { get; set; } = null!;

        string Title { get; set; } = null!;
        string StartIcon = null!;
        string ButtonSubmitText = null!;

        DateTime? BirthDate
        {
            get => UserCopy.BirthDate == DateTime.MinValue ? null : UserCopy.BirthDate;
            set { if (value != null) { UserCopy.BirthDate = value.Value; } }
        }

        protected override void OnParametersSet()
        {
            bool IsValid = false;

            if (User == null)
            {
                UserCopy = new UsersDto { Id = -1 };
                Title = "Добавление партнёра";
                StartIcon = Icons.Material.Outlined.Add;
                ButtonSubmitText = "Добавить";
            }
            else
            {
                Title = $"Редактирование партнёра - {User.Name}";
                StartIcon = Icons.Material.Outlined.Check;
                ButtonSubmitText = "Сохранить";
                UserCopy = User.DeepCopy<UsersDto>()!;
                IsValid = true;
            }

            dialogProperties = new Dictionary<short, DialogProperty>
            {
                { 1, new DialogProperty { Items = new Dictionary<string, bool>
                        {
                            { nameof(UserCopy.Name), IsValid },
                            { nameof(UserCopy.BirthDate), IsValid },
                            { nameof(UserCopy.Height), IsValid },
                            { nameof(UserCopy.Weight), IsValid }
                        }
                    }
                }
            };
        }

        #region /// Валидаторы ///
        Color NameIconColor = Color.Default;
        string? NameValidator(string name)
        {
            string? errorMessage = null;
            if (string.IsNullOrWhiteSpace(name))
                errorMessage = $"Укажите имя партнёра";

            if (string.IsNullOrWhiteSpace(name) || name.Length < StaticData.DB_USERS_NAME_MIN)
                errorMessage = $"Имя должно содержать {StaticData.DB_USERS_NAME_MIN}-{StaticData.DB_USERS_NAME_MAX} символов";

            CheckProperties(errorMessage, nameof(UserCopy.Name), ref NameIconColor);
            return errorMessage;
        }

        Color BirthDateIconColor = Color.Default;
        string? BirthDateValidator(DateTime? birthDate)
        {
            string? errorMessage = null;
            if (birthDate == null)
                errorMessage = $"Укажите дату рождения";

            if (birthDate.HasValue && (birthDate < DateTime.Now.AddYears(-75) || birthDate > DateTime.Now.AddYears(-20)))
                errorMessage = $"Возраст от 20 до 75 лет";

            CheckProperties(errorMessage, nameof(UserCopy.BirthDate), ref BirthDateIconColor);
            return errorMessage;
        }

        void BirthDateChanged(DateTime? birthDate) =>
            BirthDate = birthDate;

        Color HeightIconColor = Color.Default;
        string? HeightValidator(short height)
        {
            string? errorMessage = null;
            if (height < StaticData.DB_USERS_HEIGHT_MIN || height > StaticData.DB_USERS_HEIGHT_MAX)
                errorMessage = $"Рост в пределах {StaticData.DB_USERS_HEIGHT_MIN}-{StaticData.DB_USERS_HEIGHT_MAX} см";

            CheckProperties(errorMessage, nameof(UserCopy.Height), ref HeightIconColor);
            return errorMessage;
        }

        Color WeightIconColor = Color.Default;
        string? WeightValidator(short height)
        {
            string? errorMessage = null;

            if (height < StaticData.DB_USERS_WEIGHT_MIN || height > StaticData.DB_USERS_WEIGHT_MAX)
                errorMessage = $"Вес в пределах {StaticData.DB_USERS_WEIGHT_MIN}-{StaticData.DB_USERS_WEIGHT_MAX} кг";

            CheckProperties(errorMessage, nameof(UserCopy.Weight), ref WeightIconColor);
            return errorMessage;
        }

        void CheckProperties(string? errorMessage, string property, ref Color iconColor)
        {
            if (errorMessage == null)
            {
                dialogProperties[1].Items[property] = true;
                iconColor = Color.Success;
            }
            else
            {
                dialogProperties[1].Items[property] = false;
                iconColor = Color.Error;
            }
            StateHasChanged();
        }
        #endregion


        void Submit() => MudDialog.Close(DialogResult.Ok(UserCopy));

        void Cancel() => MudDialog.Cancel();
    }
}
