using Data.Dto.Requests;
using Data.Models;
using Dapper;
using Data.Entities;
using PhotoSauce.MagicScaler;
using System.Text.RegularExpressions;
using WebAPI.Exceptions;
using WebAPI.Models;

namespace WebAPI.Extensions
{
    public static partial class RequestsExtensions
    {
        public static async Task ValidateAsync(this RegisterAccountRequestDto request, UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new BadRequestException("Укажите Ваш email!");

            if (!Regex.IsMatch(request.Email, @"^[a-z0-9_\.-]{1,32}@[a-z0-9\.-]{1,32}\.[a-z]{2,8}$"))
                throw new BadRequestException("Проверьте корректность email!");

            if (request.Email.Length < StaticData.DB_ACCOUNTS_EMAIL_MIN || request.Email.Length > StaticData.DB_ACCOUNTS_EMAIL_MAX)
                throw new BadRequestException($"Длина email должна быть от {StaticData.DB_ACCOUNTS_EMAIL_MIN} до {StaticData.DB_ACCOUNTS_EMAIL_MAX} символов!");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new BadRequestException("Заполните имя учётной записи!");

            if (request.Name.Length < StaticData.DB_ACCOUNTS_NAME_MIN || request.Name.Length > StaticData.DB_ACCOUNTS_NAME_MAX)
                throw new BadRequestException($"Длина имени должна быть от {StaticData.DB_ACCOUNTS_NAME_MIN} до {StaticData.DB_ACCOUNTS_NAME_MAX} символов!");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new BadRequestException("Заполните поле с паролем!");

            if (string.IsNullOrWhiteSpace(request.Password2))
                throw new BadRequestException("Заполните поле с дубликатом пароля!");

            if (request.Password.Length < StaticData.DB_ACCOUNTS_PASSWORD_MIN || request.Password.Length > StaticData.DB_ACCOUNTS_PASSWORD_MAX)
                throw new BadRequestException($"Длина пароля должна быть от {StaticData.DB_ACCOUNTS_PASSWORD_MIN} до {StaticData.DB_ACCOUNTS_PASSWORD_MAX} символов!");

            if (request.Password != request.Password2)
                throw new BadRequestException("Пароли не совпадают!");

            if (request.Country == null || request.Country.Region == null || request.Country.Region.Id == 0)
                throw new BadRequestException("Вы не указали регион проживания!");

            if (request.Users != null && request.Users.Count > 2)
                throw new BadRequestException("Можно добавить не более 2-х партнёров!");

            if (request.Users == null || request.Users.Count == 0)
                throw new BadRequestException("Вы не добавили ни одного партнёра в аккаунт!");

            foreach (var user in request.Users)
            {
                if (string.IsNullOrWhiteSpace(user.Name))
                    throw new BadRequestException("Не указано имя у одного из партнёров!");

                if (user.Name.Length < StaticData.DB_USERS_NAME_MIN || user.Name.Length > StaticData.DB_USERS_NAME_MAX)
                    throw new BadRequestException($"Длина имени у {user.Name} должна быть от {StaticData.DB_USERS_NAME_MIN} до {StaticData.DB_USERS_NAME_MAX} символов!");

                if (user.Height < StaticData.DB_USERS_HEIGHT_MIN || user.Height > StaticData.DB_USERS_HEIGHT_MAX)
                    throw new BadRequestException($"Рост у {user.Name} должна быть от {StaticData.DB_USERS_HEIGHT_MIN} до {StaticData.DB_USERS_HEIGHT_MAX} см!");

                if (user.Weight < StaticData.DB_USERS_WEIGHT_MIN || user.Weight > StaticData.DB_USERS_WEIGHT_MAX)
                    throw new BadRequestException($"Вес у {user.Name} должен быть от {StaticData.DB_USERS_WEIGHT_MIN} до {StaticData.DB_USERS_WEIGHT_MAX} кг!");

                if (user.Gender < 0 || user.Gender > 1)
                    throw new BadRequestException($"Укажите пол у {user.Name}!");

                if (user.BirthDate.Date < DateTime.Now.AddYears(-75).Date || user.BirthDate.Date > DateTime.Now.AddYears(-20).Date)
                    throw new BadRequestException($"Возраст у {user.Name} должен быть от 20 до 75 лет!");
            }

            var sql = "SELECT TOP 1 Id FROM Regions WHERE Id = @Id";
            var regionId = await unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<int?>(sql, new { request.Country.Region.Id })
                ?? throw new BadRequestException($"Указанный регион (id: {request.Country.Region.Id}) не найден в базе данных!");

            sql = "SELECT TOP 1 Id FROM Identities WHERE Email = @Email";
            var resultEmail = await unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<int?>(sql, new { request.Email });
            if (resultEmail != null)
                throw new BadRequestException($"Аккаунт с email {request.Email} уже зарегистрирован! Укажите другой адрес или запросите пароль на email.");

            sql = "SELECT TOP 1 Id FROM Accounts WHERE Name = @Name";
            var resultName = await unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<int?>(sql, new { request.Name });
            if (resultName != null)
                throw new BadRequestException($"Аккаунт с именем {request.Name} уже зарегистрирован!");

            if (!request.AcceptTerms)
                throw new BadRequestException("Вы не приняли условия пользования сайтом!");
        }

        public static async Task<int> AddAccountAsync(this RegisterAccountRequestDto request, UnitOfWork unitOfWork)
        {
            var sql = "INSERT INTO Accounts " +
                $"({nameof(AccountsEntity.Name)}, {nameof(AccountsEntity.RegionId)}) " +
                "VALUES " +
                $"(@{nameof(AccountsEntity.Name)}, @{nameof(AccountsEntity.RegionId)}) " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT)";
            var AccountId = await unitOfWork.SqlConnection.QuerySingleAsync<int>(sql, new { request.Name, RegionId = request.Country.Region.Id }, 
                transaction: unitOfWork.SqlTransaction);

            sql = "INSERT INTO Identities " +
                $"({nameof(IdentitiesEntity.AccountId)}, {nameof(IdentitiesEntity.Email)}, {nameof(IdentitiesEntity.Password)}) " +
                "VALUES " +
                $"(@{nameof(IdentitiesEntity.AccountId)}, @{nameof(IdentitiesEntity.Email)}, @{nameof(IdentitiesEntity.Password)})";
            await unitOfWork.SqlConnection.ExecuteAsync(sql, new { AccountId, request.Email, request.Password },
                transaction: unitOfWork.SqlTransaction);

            sql = "INSERT INTO Informings " +
                $"({nameof(InformingsEntity.AccountId)}, {nameof(InformingsEntity.IsNewNotification)}, {nameof(InformingsEntity.IsNewMessage)}) " +
                "VALUES " +
                $"(@{nameof(InformingsEntity.AccountId)}, @{nameof(InformingsEntity.IsNewNotification)}, @{nameof(InformingsEntity.IsNewMessage)})";
            await unitOfWork.SqlConnection.ExecuteAsync(sql, new { AccountId, request.Informings.IsNewNotification, request.Informings.IsNewMessage },
                transaction: unitOfWork.SqlTransaction);

            return AccountId;
        }

        public static async Task AddUsersAsync(this RegisterAccountRequestDto request, UnitOfWork unitOfWork)
        {
            foreach (var u in request.Users)
            {
                var sql = "INSERT INTO Users " +
                    $"({nameof(UsersEntity.Name)}, {nameof(UsersEntity.Height)}, {nameof(UsersEntity.Weight)}, {nameof(UsersEntity.BirthDate)}, {nameof(UsersEntity.Gender)}, {nameof(UsersEntity.AccountId)}) " +
                    "VALUES " +
                    $"(@{nameof(UsersEntity.Name)}, @{nameof(UsersEntity.Height)}, @{nameof(UsersEntity.Weight)}, @{nameof(UsersEntity.BirthDate)}, @{nameof(UsersEntity.Gender)}, @{nameof(UsersEntity.AccountId)})";
                await unitOfWork.SqlConnection.ExecuteAsync(sql, new { u.Name, u.Height, u.Weight, u.BirthDate, u.Gender, AccountId = request.Id }, transaction: unitOfWork.SqlTransaction);
            }
        }

        public static async Task AddHobbiesAsync(this RegisterAccountRequestDto request, UnitOfWork unitOfWork)
        {
            foreach (var h in request.Hobbies!)
            {
                var sql = "INSERT INTO HobbiesForAccounts " +
                    $"({nameof(HobbiesForAccountsEntity.AccountId)}, {nameof(HobbiesForAccountsEntity.HobbyId)}) " +
                    "VALUES " +
                    $"(@{nameof(HobbiesForAccountsEntity.AccountId)}, @{nameof(HobbiesForAccountsEntity.HobbyId)})";
                await unitOfWork.SqlConnection.ExecuteAsync(sql, new { AccountId = request.Id, HobbyId = h.Id }, transaction: unitOfWork.SqlTransaction);
            }
        }

        public static async Task AddWishListAsync(this RegisterAccountRequestDto request, UnitOfWork unitOfWork)
        {
            var sql = $"INSERT INTO AccountsWishLists ({nameof(AccountsWishLists.Comment)}, {nameof(AccountsWishLists.AccountId)}) " +
                $"VALUES (@Comment, @AccountId)";
            await unitOfWork.SqlConnection.ExecuteAsync(sql, new { Comment = "Привет!", AccountId = request.Id }, transaction: unitOfWork.SqlTransaction);
        }

        public static async Task AddPhotosAsync(this RegisterAccountRequestDto request, UnitOfWork unitOfWork)
        {
            string sql;

            if (request.Photos != null)
            {
                foreach (var photo in request.Photos)
                {
                    // Есть ли фото во временном каталоге?
                    if (Directory.Exists($"{StaticData.TempPhotosDir}/{photo.Guid}"))
                    {
                        // Фото добавили, затем сразу удалили, а потом сохраняют. Значит, фото можно не обрабатывать.
                        if (!photo.IsDeleted)
                        {
                            Directory.CreateDirectory($"{StaticData.AccountsPhotosDir}/{request.Id}/{photo.Guid}");
                            var sourceFileName = $"{StaticData.TempPhotosDir}/{photo.Guid}/original.jpg";

                            //foreach (var image in StaticData.Images)
                            //{
                            //    var destFileName = $@"{StaticData.AccountsPhotosDir}/{request.Id}/{photo.Guid}/{image.Key}.jpg";

                            //    MemoryStream output = new MemoryStream(300000);
                            //    MagicImageProcessor.ProcessImage(sourceFileName, output, image.Value);
                            //    File.WriteAllBytes(destFileName, output.ToArray());
                            //}

                            //sql = "INSERT INTO PhotosForAccounts " +
                            //    $"({nameof(PhotosForAccountsEntity.Guid)}, {nameof(PhotosForAccountsEntity.RelatedId)}, {nameof(PhotosForAccountsEntity.Comment)}, {nameof(PhotosForAccountsEntity.IsAvatar)}) " +
                            //    "VALUES " +
                            //    $"(@{nameof(PhotosForAccountsEntity.Guid)}, @{nameof(PhotosForAccountsEntity.RelatedId)}, @{nameof(PhotosForAccountsEntity.Comment)}, @{nameof(PhotosForAccountsEntity.IsAvatar)});" +
                            //    $"SELECT CAST(SCOPE_IDENTITY() AS INT)";
                            //var newId = await unitOfWork.SqlConnection.QuerySingleAsync<int>(sql, new { photo.Guid, RelatedId = request.Id, photo.Comment, photo.IsAvatar }, transaction: unitOfWork.SqlTransaction);
                        }
                        Directory.Delete($"{StaticData.TempPhotosDir}/{photo.Guid}", true);
                    }
                }
            }
        }

    }
}
