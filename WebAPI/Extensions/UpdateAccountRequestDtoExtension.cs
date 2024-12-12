using Common.Dto.Requests;
using Common.Models;
using Dapper;
using DataContext.Entities;
using PhotoSauce.MagicScaler;
using System.Text.RegularExpressions;
using WebAPI.Exceptions;
using WebAPI.Models;

namespace WebAPI.Extensions
{
    public static partial class RequestsExtensions
    {
        public static async Task ValidateAsync(this UpdateAccountRequestDto request, UnitOfWork unitOfWork)
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

            if (request.Users == null || request.Users.Where(u => u.IsDeleted == false).Count() == 0)
                throw new BadRequestException("Вы не добавили ни одного партнёра в аккаунт!");

            if (request.Users != null && request.Users.Where(w => w.IsDeleted == false).Count() > 2)
                throw new BadRequestException("Можно добавить не более 2-х партнёров!");

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                if (request.Password != request.Password2)
                    throw new BadRequestException("Пароль и повтор пароля не совпадают!");
            }

            foreach (var user in request.Users!.Where(u => u.IsDeleted == false))
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

            if (request.Country == null || request.Country.Region == null || request.Country.Region.Id == 0)
                throw new BadRequestException("Вы не указали регион проживания!");

            var sql = $"SELECT TOP 1 Id FROM Identities WHERE {nameof(IdentitiesEntity.Email)} = @{nameof(IdentitiesEntity.Email)} AND {nameof(IdentitiesEntity.AccountId)} <> @{nameof(IdentitiesEntity.AccountId)}";
            if ((await unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<int?>(sql, new { request.Email, unitOfWork.AccountId })) != null)
                throw new BadRequestException($"Аккаунт с email {request.Email} уже зарегистрирован!");

            sql = $"SELECT TOP 1 Id FROM Accounts WHERE {nameof(AccountsEntity.Name)} = @{nameof(AccountsEntity.Name)} AND Id <> @AccountId";
            if ((await unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<int?>(sql, new { request.Name, unitOfWork.AccountId })) != null)
                throw new BadRequestException($"Аккаунт с именем {request.Name} уже зарегистрирован!");
        }

        public static async Task UpdateAccountAsync(this UpdateAccountRequestDto request, UnitOfWork unitOfWork)
        {
            var sql = $"UPDATE Accounts SET " +
                $"{nameof(AccountsEntity.Name)} = @{nameof(request.Name)}, " +
                $"{nameof(AccountsEntity.RegionId)} = @{nameof(AccountsEntity.RegionId)} " +
                "WHERE Id = @AccountId";
            await unitOfWork.SqlConnection.ExecuteAsync(sql, new { request.Email, request.Name, RegionId = request.Country.Region.Id, unitOfWork.AccountId }, transaction: unitOfWork.SqlTransaction);

            sql = $"UPDATE Identities SET " +
                $"{nameof(IdentitiesEntity.Email)} = @{nameof(request.Email)}, " +
                $"{nameof(IdentitiesEntity.Password)} = @{nameof(request.Password)} " +
                "WHERE AccountId = @AccountId";
            await unitOfWork.SqlConnection.ExecuteAsync(sql, new { request.Email, request.Password, unitOfWork.AccountId }, transaction: unitOfWork.SqlTransaction);

            sql = $"UPDATE Informings SET " +
                $"{nameof(InformingsEntity.IsNewNotification)} = @{nameof(request.Informings.IsNewNotification)}, " +
                $"{nameof(InformingsEntity.IsNewMessage)} = @{nameof(request.Informings.IsNewMessage)} " +
                "WHERE AccountId = @AccountId";
            await unitOfWork.SqlConnection.ExecuteAsync(sql, new { request.Informings.IsNewNotification, request.Informings.IsNewMessage, unitOfWork.AccountId }, transaction: unitOfWork.SqlTransaction);
        }

        public static async Task UpdateUsersAsync(this UpdateAccountRequestDto request, UnitOfWork unitOfWork)
        {
            foreach (var user in request.Users)
            {
                // Добавление
                if (user.Id == -1)
                {
                    var sql = $"INSERT INTO Users ({nameof(UsersEntity.Name)}, {nameof(UsersEntity.Height)}, {nameof(UsersEntity.Weight)}, {nameof(UsersEntity.BirthDate)}, {nameof(UsersEntity.About)}, {nameof(UsersEntity.Gender)}, {nameof(UsersEntity.AccountId)}) " +
                        "VALUES " +
                        $"(@{nameof(UsersEntity.Name)}, @{nameof(UsersEntity.Height)}, @{nameof(UsersEntity.Weight)}, @{nameof(UsersEntity.BirthDate)}, @{nameof(UsersEntity.About)}, @{nameof(UsersEntity.Gender)}, @AccountId)";
                    await unitOfWork.SqlConnection.ExecuteAsync(sql, new { user.Name, user.Height, user.Weight, user.BirthDate, user.About, user.Gender, unitOfWork.AccountId }, transaction: unitOfWork.SqlTransaction);
                }
                // Обновление / Удаление
                else
                {
                    var sql = $"UPDATE Users SET " +
                        $"{nameof(UsersEntity.BirthDate)} = @{nameof(user.BirthDate)}, " +
                        $"{nameof(UsersEntity.Name)} = @{nameof(user.Name)}, " +
                        $"{nameof(UsersEntity.Gender)} = @{nameof(user.Gender)}, " +
                        $"{nameof(UsersEntity.Height)} = @{nameof(user.Height)}, " +
                        $"{nameof(UsersEntity.Weight)} = @{nameof(user.Weight)}, " +
                        $"{nameof(UsersEntity.About)} = @{nameof(user.About)}, " +
                        $"{nameof(UsersEntity.IsDeleted)} = @{nameof(user.IsDeleted)} " +
                        $"WHERE Id = @Id AND AccountId = @AccountId";
                    await unitOfWork.SqlConnection.ExecuteAsync(sql, new { user.Id, unitOfWork.AccountId, user.BirthDate, user.Name, user.Gender, user.Height, user.Weight, user.About, user.IsDeleted }, transaction: unitOfWork.SqlTransaction);
                }
            }
        }

        public static async Task UpdateHobbiesAsync(this UpdateAccountRequestDto request, UnitOfWork unitOfWork)
        {
            if (request.Hobbies != null)
            {
                var p = new DynamicParameters();
                p.Add("@AccountId", unitOfWork.AccountId);
                p.Add("@HobbiesIds", string.Join(",", request.Hobbies.Select(s => s.Id)));
                await unitOfWork.SqlConnection.ExecuteAsync("UpdateHobbiesForAccounts_sp", p, commandType: System.Data.CommandType.StoredProcedure, transaction: unitOfWork.SqlTransaction);
            }
        }

        public static async Task UpdatePasswordAsync(this UpdateAccountRequestDto request, UnitOfWork unitOfWork)
        {
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var sql = $"UPDATE Identities SET {nameof(IdentitiesEntity.Password)} = @{nameof(IdentitiesEntity.Password)} WHERE {nameof(IdentitiesEntity.Id)} = @AccountId";
                await unitOfWork.SqlConnection.ExecuteAsync(sql, new { Password = request.Password2, unitOfWork.AccountId }, transaction: unitOfWork.SqlTransaction);
            }
        }

        public static async Task UpdatePhotosAsync(this UpdateAccountRequestDto request, UnitOfWork unitOfWork)
        {
            string sql;

            if (request.Photos != null)
            {
                // Снимем все галки аватара (на случай, если параллельно на другом устройстве редактируют фото)
                sql = $"UPDATE PhotosForAccounts SET {nameof(PhotosForAccountsEntity.IsAvatar)} = 0 " +
                    $"WHERE {nameof(PhotosForAccountsEntity.RelatedId)} = @{nameof(PhotosForAccountsEntity.RelatedId)}";
                var result = await unitOfWork.SqlConnection.ExecuteAsync(sql, new { RelatedId = request.Id }, transaction: unitOfWork.SqlTransaction);

                foreach (var photo in request.Photos)
                {
                    // Обновление существующего фото
                    if (photo.Id != 0)
                    {
                        sql = $"UPDATE PhotosForAccounts SET " +
                            $"{nameof(PhotosForAccountsEntity.Comment)} = @{nameof(PhotosForAccountsEntity.Comment)}, " +
                            $"{nameof(PhotosForAccountsEntity.IsAvatar)} = @{nameof(PhotosForAccountsEntity.IsAvatar)}, " +
                            $"{nameof(PhotosForAccountsEntity.IsDeleted)} = @{nameof(PhotosForAccountsEntity.IsDeleted)} " +
                            $"WHERE Id = @Id AND {nameof(PhotosForAccountsEntity.RelatedId)} = @{nameof(PhotosForAccountsEntity.RelatedId)}";
                        result = await unitOfWork.SqlConnection.ExecuteAsync(sql,
                            new { photo.Id, RelatedId = request.Id, photo.IsAvatar, photo.Comment, photo.IsDeleted },
                            transaction: unitOfWork.SqlTransaction);
                    }
                    // Добавление нового фото
                    else
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

                                sql = "INSERT INTO PhotosForAccounts " +
                                    $"({nameof(PhotosForAccountsEntity.Guid)}, {nameof(PhotosForAccountsEntity.RelatedId)}, {nameof(PhotosForEventsEntity.Comment)}, {nameof(PhotosForAccountsEntity.IsAvatar)}) " +
                                    "VALUES " +
                                    $"(@{nameof(PhotosForAccountsEntity.Guid)}, @{nameof(PhotosForAccountsEntity.RelatedId)}, @{nameof(PhotosForEventsEntity.Comment)}, @{nameof(PhotosForAccountsEntity.IsAvatar)});" +
                                    $"SELECT CAST(SCOPE_IDENTITY() AS INT)";
                                var newId = await unitOfWork.SqlConnection.QuerySingleAsync<int>(sql, new { photo.Guid, RelatedId = request.Id, photo.Comment, photo.IsAvatar }, transaction: unitOfWork.SqlTransaction);
                            }
                            Directory.Delete($"{StaticData.TempPhotosDir}/{photo.Guid}", true);
                        }
                    }
                }
            }
        }
    }
}