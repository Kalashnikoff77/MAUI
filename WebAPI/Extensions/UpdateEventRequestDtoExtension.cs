using Common.Dto.Requests;
using Common.Extensions;
using Common.Models;
using Dapper;
using DataContext.Entities;
using PhotoSauce.MagicScaler;
using WebAPI.Exceptions;
using WebAPI.Models;

namespace WebAPI.Extensions
{
    public static partial class RequestsExtensions
    {
        /// <summary>
        /// Валидация мероприятия
        /// </summary>
        public static void Validate(this UpdateEventRequestDto request, UnitOfWork unitOfWork)
        {
            if (request.Event == null)
                throw new BadRequestException("Пустой запрос");

            if (string.IsNullOrWhiteSpace(request.Event.Name))
                throw new BadRequestException("Вы не указали название мероприятия!");

            if (request.Event.Name.Length < StaticData.DB_EVENT_NAME_MIN || request.Event.Name.Length > StaticData.DB_EVENT_NAME_MAX)
                throw new BadRequestException($"Длина названия должна быть от {StaticData.DB_EVENT_NAME_MIN} до {StaticData.DB_EVENT_NAME_MAX} символов!");

            if (request.Event.Country?.Region.Id == 0)
                throw new BadRequestException("Вы не указали регион проведения мероприятия!");

            if (string.IsNullOrWhiteSpace(request.Event.Address))
                throw new BadRequestException("Вы не указали адрес проведения мероприятия!");

            if (request.Event.Address.Length < StaticData.DB_EVENT_ADDRESS_MIN || request.Event.Address.Length > StaticData.DB_EVENT_ADDRESS_MAX)
                throw new BadRequestException($"Длина адреса должна быть от {StaticData.DB_EVENT_ADDRESS_MIN} до {StaticData.DB_EVENT_ADDRESS_MAX} символов!");

            if (request.Event.Schedule == null || request.Event.Schedule.Count == 0)
                throw new BadRequestException($"Добавьте минимум одно расписание!");

            if (request.Event.Photos == null || request.Event.Photos.Count == 0)
                throw new BadRequestException($"Добавьте минимум одно фото!");
        }

        /// <summary>
        /// Обновление мероприятия
        /// </summary>
        public static async Task<int> UpdateEventAsync(this UpdateEventRequestDto request, UnitOfWork unitOfWork)
        {
            request.Event.Description = request.Event.Description.RemoveEmptyLines();

            var sql = $"UPDATE Events SET " +
                $"{nameof(EventsEntity.Name)} = @{nameof(EventsEntity.Name)}, " +
                $"{nameof(EventsEntity.Description)} = @{nameof(EventsEntity.Description)}, " +
                $"{nameof(EventsEntity.RegionId)} = @{nameof(EventsEntity.RegionId)}, " +
                $"{nameof(EventsEntity.Address)} = @{nameof(EventsEntity.Address)}, " +
                $"{nameof(EventsEntity.MaxMen)} = @{nameof(EventsEntity.MaxMen)}, " +
                $"{nameof(EventsEntity.MaxWomen)} = @{nameof(EventsEntity.MaxWomen)}, " +
                $"{nameof(EventsEntity.MaxPairs)} = @{nameof(EventsEntity.MaxPairs)} " +
                $"WHERE Id = @Id AND {nameof(EventsEntity.AdminId)} = @AccountId";

            var result = await unitOfWork.SqlConnection.ExecuteAsync(sql,
                new
                {
                    request.Event.Id,
                    request.Event.Name,
                    request.Event.Description,
                    RegionId = request.Event.Country!.Region.Id,
                    request.Event.Address,
                    request.Event.MaxMen,
                    request.Event.MaxWomen,
                    request.Event.MaxPairs,
                    unitOfWork.AccountId
                },
                transaction: unitOfWork.SqlTransaction);

            return result;
        }

        /// <summary>
        /// Обновление расписания мероприятия
        /// </summary>
        public static async Task UpdateSchedulesAsync(this UpdateEventRequestDto request, UnitOfWork unitOfWork)
        {
            string sql;

            if (request.Event.Schedule != null)
            {
                foreach (var schedule in request.Event.Schedule)
                {
                    if (schedule.Features == null)
                        throw new BadRequestException($"Не выбрана ни одна услуга!");

                    if (string.IsNullOrWhiteSpace(schedule.Description) || schedule.Description.Length < StaticData.DB_EVENT_DESCRIPTION_MIN)
                        throw new BadRequestException($"Кол-во символов в описании расписания должно быть минимум {StaticData.DB_EVENT_DESCRIPTION_MIN}!");

                    schedule.Description = schedule.Description?.RemoveEmptyLines();

                    // Добавление нового расписания с услугами
                    if (schedule.Id == 0)
                    {
                        sql = $"INSERT INTO SchedulesForEvents (" +
                            $"{nameof(SchedulesForEventsEntity.EventId)}, " +
                            $"{nameof(SchedulesForEventsEntity.Description)}, " +
                            $"{nameof(SchedulesForEventsEntity.StartDate)}, " +
                            $"{nameof(SchedulesForEventsEntity.EndDate)}, " +
                            $"{nameof(SchedulesForEventsEntity.CostMan)}, " +
                            $"{nameof(SchedulesForEventsEntity.CostWoman)}, " +
                            $"{nameof(SchedulesForEventsEntity.CostPair)}" +
                            $") VALUES (" +
                            $"@{nameof(SchedulesForEventsEntity.EventId)}, " +
                            $"@{nameof(SchedulesForEventsEntity.Description)}, " +
                            $"@{nameof(SchedulesForEventsEntity.StartDate)}, " +
                            $"@{nameof(SchedulesForEventsEntity.EndDate)}, " +
                            $"@{nameof(SchedulesForEventsEntity.CostMan)}, " +
                            $"@{nameof(SchedulesForEventsEntity.CostWoman)}, " +
                            $"@{nameof(SchedulesForEventsEntity.CostPair)});" +
                            $"SELECT CAST(SCOPE_IDENTITY() AS INT)";

                        var insertedScheduleId = await unitOfWork.SqlConnection.QuerySingleAsync<int>(sql,
                            new { EventId = request.Event.Id, schedule.Description, schedule.StartDate, schedule.EndDate, schedule.CostMan, schedule.CostWoman, schedule.CostPair },
                            transaction: unitOfWork.SqlTransaction);

                        // Доп. услуги расписания (features)
                        var p = new DynamicParameters();
                        p.Add("@ScheduleId", insertedScheduleId);
                        p.Add("@FeaturesIds", string.Join(",", schedule.Features.Select(s => s.Id)));
                        await unitOfWork.SqlConnection.ExecuteAsync("UpdateFeaturesForSchedule_sp", p, commandType: System.Data.CommandType.StoredProcedure, transaction: unitOfWork.SqlTransaction);
                    }

                    // Обновление расписания с услугами
                    else
                    {
                        sql = $"UPDATE SchedulesForEvents SET " +
                            $"{nameof(SchedulesForEventsEntity.Description)} = @{nameof(SchedulesForEventsEntity.Description)}, " +
                            $"{nameof(SchedulesForEventsEntity.StartDate)} = @{nameof(SchedulesForEventsEntity.StartDate)}, " +
                            $"{nameof(SchedulesForEventsEntity.EndDate)} = @{nameof(SchedulesForEventsEntity.EndDate)}, " +
                            $"{nameof(SchedulesForEventsEntity.CostMan)} = @{nameof(SchedulesForEventsEntity.CostMan)}, " +
                            $"{nameof(SchedulesForEventsEntity.CostWoman)} = @{nameof(SchedulesForEventsEntity.CostWoman)}, " +
                            $"{nameof(SchedulesForEventsEntity.CostPair)} = @{nameof(SchedulesForEventsEntity.CostPair)}, " +
                            $"{nameof(SchedulesForEventsEntity.IsDeleted)} = @{nameof(SchedulesForEventsEntity.IsDeleted)} " +
                            $"WHERE Id = @Id AND EventId = @{nameof(SchedulesForEventsEntity.EventId)}";

                        var result = await unitOfWork.SqlConnection.ExecuteAsync(sql,
                            new { schedule.Id, EventId = request.Event.Id, schedule.Description, schedule.StartDate, schedule.EndDate, schedule.CostMan, schedule.CostWoman, schedule.CostPair, schedule.IsDeleted },
                            transaction: unitOfWork.SqlTransaction);

                        // Доп. услуги расписания (features)
                        var p = new DynamicParameters();
                        p.Add("@ScheduleId", schedule.Id);
                        p.Add("@FeaturesIds", string.Join(",", schedule.Features.Select(s => s.Id)));
                        await unitOfWork.SqlConnection.ExecuteAsync("UpdateFeaturesForSchedule_sp", p, commandType: System.Data.CommandType.StoredProcedure, transaction: unitOfWork.SqlTransaction);
                    }
                }
            }
        }

        /// <summary>
        /// Обновление фото мероприятия
        /// </summary>
        public static async Task UpdatePhotosAsync(this UpdateEventRequestDto request, UnitOfWork unitOfWork)
        {
            string sql;

            if (request.Event.Photos != null)
            {
                // Снимем все галки аватара (на случай, если параллельно на другом устройстве редактируют фото)
                sql = $"UPDATE PhotosForEvents SET {nameof(PhotosForEventsEntity.IsAvatar)} = 0 " +
                    $"WHERE {nameof(PhotosForEventsEntity.RelatedId)} = @{nameof(PhotosForEventsEntity.RelatedId)}";
                var result = await unitOfWork.SqlConnection.ExecuteAsync(sql, new { RelatedId = request.Event.Id }, transaction: unitOfWork.SqlTransaction);

                foreach (var photo in request.Event.Photos)
                {
                    // Обновление существующего фото
                    if (photo.Id != 0)
                    {
                        sql = $"UPDATE PhotosForEvents SET " +
                            $"{nameof(PhotosForEventsEntity.Comment)} = @{nameof(PhotosForEventsEntity.Comment)}, " +
                            $"{nameof(PhotosForEventsEntity.IsAvatar)} = @{nameof(PhotosForEventsEntity.IsAvatar)}, " +
                            $"{nameof(PhotosForEventsEntity.IsDeleted)} = @{nameof(PhotosForEventsEntity.IsDeleted)} " +
                            $"WHERE Id = @Id AND {nameof(PhotosForEventsEntity.RelatedId)} = @{nameof(PhotosForEventsEntity.RelatedId)}";
                        result = await unitOfWork.SqlConnection.ExecuteAsync(sql,
                            new { photo.Id, RelatedId = request.Event.Id, photo.IsAvatar, photo.Comment, photo.IsDeleted },
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
                                Directory.CreateDirectory($"{StaticData.EventsPhotosDir}/{request.Event.Id}/{photo.Guid}");
                                var sourceFileName = $"{StaticData.TempPhotosDir}/{photo.Guid}/original.jpg";

                                //foreach (var image in StaticData.Images)
                                //{
                                //    var destFileName = $@"{StaticData.EventsPhotosDir}/{request.Event.Id}/{photo.Guid}/{image.Key}.jpg";

                                //    MemoryStream output = new MemoryStream(300000);
                                //    MagicImageProcessor.ProcessImage(sourceFileName, output, image.Value);
                                //    File.WriteAllBytes(destFileName, output.ToArray());
                                //}

                                sql = "INSERT INTO PhotosForEvents " +
                                    $"({nameof(PhotosForEventsEntity.Guid)}, {nameof(PhotosForEventsEntity.RelatedId)}, {nameof(PhotosForEventsEntity.Comment)}, {nameof(PhotosForEventsEntity.IsAvatar)}) " +
                                    "VALUES " +
                                    $"(@{nameof(PhotosForEventsEntity.Guid)}, @{nameof(PhotosForEventsEntity.RelatedId)}, @{nameof(PhotosForEventsEntity.Comment)}, @{nameof(PhotosForEventsEntity.IsAvatar)});" +
                                    $"SELECT CAST(SCOPE_IDENTITY() AS INT)";
                                var newId = await unitOfWork.SqlConnection.QuerySingleAsync<int>(sql, new { photo.Guid, RelatedId = request.Event.Id, photo.Comment, photo.IsAvatar }, transaction: unitOfWork.SqlTransaction);
                            }
                            Directory.Delete($"{StaticData.TempPhotosDir}/{photo.Guid}", true);
                        }
                    }
                }
            }
        }
    }
}
