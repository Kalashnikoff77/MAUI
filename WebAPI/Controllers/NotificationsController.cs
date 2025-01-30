using AutoMapper;
using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Dapper;
using Data.Entities;
using Data.Entities.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebAPI.Exceptions;
using Data.Dto;
using Data.Models;
using Data.Dto.Sp;
using Data.Entities.Sp;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : MyControllerBase
    {
        public NotificationsController(IMapper mapper, IConfiguration configuration, IMemoryCache cache) : base(configuration, mapper, cache) { }

        // Получить одно или все уведомления
        [Route("Get"), HttpPost, Authorize]
        public async Task<GetNotificationsResponseDto> GetAsync(GetNotificationsRequestDto request)
        {
            AuthenticateUser();

            var response = new GetNotificationsResponseDto();

            if (request.NotificationId.HasValue && request.NotificationId.Value > 0)
            {
                NotificationsEntity? result;
                var sql = $"SELECT TOP 1 * FROM Notifications " +
                    $"WHERE (({nameof(NotificationsEntity.SenderId)} = @AccountId AND {nameof(NotificationsEntity.RecipientId)} = @RecipientId) " +
                    $"OR ({nameof(NotificationsEntity.SenderId)} = @RecipientId AND {nameof(NotificationsEntity.RecipientId)} = @AccountId))";
                result = await _unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<NotificationsEntity>(sql, new { _unitOfWork.AccountId, request.RecipientId });
                response.Notification = _unitOfWork.Mapper.Map<NotificationsDto>(result);
            }
            else
            {
                IEnumerable<NotificationsEntity> result;

                // Получим кол-во уведомлений
                var sql = $"SELECT COUNT(*) FROM Notifications " +
                    $"WHERE ({nameof(NotificationsEntity.SenderId)} = @AccountId AND {nameof(NotificationsEntity.RecipientId)} = @RecipientId) " +
                    $"OR ({nameof(NotificationsEntity.SenderId)} = @RecipientId AND {nameof(NotificationsEntity.RecipientId)} = @AccountId)";
                response.Count = await _unitOfWork.SqlConnection.QueryFirstAsync<int>(sql, new { _unitOfWork.AccountId, request.RecipientId });

                // Запрос на получение предыдущих уведомлений
                if (request.GetPreviousFromId.HasValue)
                {
                    sql = $"SELECT TOP (@Take) * FROM Notifications " +
                        $"WHERE (({nameof(NotificationsEntity.SenderId)} = @AccountId AND {nameof(NotificationsEntity.RecipientId)} = @RecipientId) " +
                        $"OR ({nameof(NotificationsEntity.SenderId)} = @RecipientId AND {nameof(NotificationsEntity.RecipientId)} = @AccountId)) " +
                        $"AND Id < {request.GetPreviousFromId} " +
                        $"ORDER BY Id DESC";
                    result = (await _unitOfWork.SqlConnection.QueryAsync<NotificationsEntity>(sql, new { _unitOfWork.AccountId, request.RecipientId, request.Take })).Reverse();
                }

                // Запрос на получение следующих сообщений
                else if (request.GetNextAfterId.HasValue)
                {
                    sql = $"SELECT TOP (@Take) * FROM Notifications " +
                        $"WHERE (({nameof(NotificationsEntity.SenderId)} = @AccountId AND {nameof(NotificationsEntity.RecipientId)} = @RecipientId) " +
                        $"OR ({nameof(NotificationsEntity.SenderId)} = @RecipientId AND {nameof(NotificationsEntity.RecipientId)} = @AccountId)) " +
                        $"AND Id > {request.GetNextAfterId} " +
                        $"ORDER BY Id ASC";
                    result = await _unitOfWork.SqlConnection.QueryAsync<NotificationsEntity>(sql, new { _unitOfWork.AccountId, request.RecipientId, request.Take });
                }

                // Запрос на получение последних сообщений (по умолчанию)
                else
                {
                    int offset = response.Count.Value > StaticData.NOTIFICATIONS_PER_BLOCK ? response.Count.Value - StaticData.NOTIFICATIONS_PER_BLOCK : 0;
                    sql = $"SELECT * FROM Notifications " +
                        $"WHERE ({nameof(NotificationsEntity.SenderId)} = @AccountId AND {nameof(NotificationsEntity.RecipientId)} = @RecipientId) " +
                        $"OR ({nameof(NotificationsEntity.SenderId)} = @RecipientId AND {nameof(NotificationsEntity.RecipientId)} = @AccountId) " +
                        $"ORDER BY Id ASC " +
                        $"OFFSET {offset} ROWS";
                    result = await _unitOfWork.SqlConnection.QueryAsync<NotificationsEntity>(sql, new { _unitOfWork.AccountId, request.RecipientId });
                }

                response.Notifications = _unitOfWork.Mapper.Map<List<NotificationsDto>>(result);

                // Получим отправителя и получателя
                var columns = GetRequiredColumns<AccountsViewEntity>();
                sql = $"SELECT TOP 2 {columns.Aggregate((a, b) => a + ", " + b)} FROM AccountsView WHERE Id = @AccountId OR Id = @RecipientId";
                var accounts = await _unitOfWork.SqlConnection.QueryAsync<AccountsViewEntity>(sql, new { _unitOfWork.AccountId, request.RecipientId });

                response.Accounts = accounts.Select(x => new KeyValuePair<int, AccountsViewDto>(x.Id, _unitOfWork.Mapper.Map<AccountsViewDto>(x))).ToDictionary();

                // Будем отмечать сообщения, как прочитанные?
                if (request.MarkAsRead)
                {
                    var ids = response.Notifications
                        .Where(w => w.RecipientId == _unitOfWork.AccountId && w.ReadDate == null)
                        .Select(s => s.Id.ToString());

                    if (ids.Any())
                    {
                        // Пометим в базе
                        await _unitOfWork.SqlConnection.ExecuteAsync($"UPDATE Notifications SET {nameof(NotificationsEntity.ReadDate)} = getdate() " +
                            $"WHERE Id IN ({ids.Aggregate((a, b) => a + ", " + b)})");
                        // Пометим в ответе
                        response.Notifications.ForEach(m => m.ReadDate = DateTime.Now);
                    }
                }
            }

            return response;
        }


        //[Route("Get"), HttpPost, Authorize]
        //public async Task<GetNotificationsResponseDto?> GetAsync(GetNotificationsRequestDto request)
        //{
        //    AuthenticateUser();

        //    var response = new GetNotificationsResponseDto();

        //    // Получим все уведомления (с фильтром)
        //    var sql = "SELECT * FROM NotificationsView " +
        //        $"WHERE {nameof(NotificationsViewEntity.RecipientId)} = @AccountId" +
        //        $"ORDER BY {nameof(NotificationsViewEntity.CreateDate)} DESC " +
        //        $"OFFSET {request.Skip} ROWS FETCH NEXT {request.Take} ROWS ONLY";
        //    var notifications = await _unitOfWork.SqlConnection.QueryAsync<NotificationsViewEntity>(sql, new { _unitOfWork.AccountId });
        //    response.Notifications = _unitOfWork.Mapper.Map<List<NotificationsViewDto>>(notifications);

        //    // Подсчитаем кол-во уведомлений (с фильтром)
        //    sql = "SELECT COUNT(*) FROM NotificationsView " +
        //        $"WHERE {nameof(NotificationsViewEntity.RecipientId)} = @AccountId";
        //    response.Count = await _unitOfWork.SqlConnection.QuerySingleAsync<int>(sql, new { _unitOfWork.AccountId });

        //    // Будем отмечать уведомления, как прочитанные?
        //    if (request.MarkAsRead && response.Notifications.Any(x => x.ReadDate == null))
        //    {
        //        var ids = response.Notifications
        //            .Where(w => w.ReadDate == null)
        //            .Select(s => s.Id.ToString());

        //        if (ids.Any())
        //            await _unitOfWork.SqlConnection.ExecuteAsync($"UPDATE Notifications SET {nameof(NotificationsEntity.ReadDate)} = getdate() " +
        //                $"WHERE Id IN ({ids.Aggregate((a, b) => a + ", " + b)})");
        //    }

        //    return response;
        //}


        [Route("GetLastNotificationsList"), HttpPost, Authorize]
        public async Task<GetLastNotificationsListResponseDto> GetLastNotificationListAsync(GetLastNotificationsListRequestDto request)
        {
            AuthenticateUser();

            var response = new GetLastNotificationsListResponseDto();

            var p = new DynamicParameters();
            p.Add("@AccountId", _unitOfWork.AccountId);
            var result = await _unitOfWork.SqlConnection.QueryAsync<LastNotificationsForAccountSpEntity>("GetLastNotificationsForAccount_sp", p, commandType: System.Data.CommandType.StoredProcedure);
            response.LastNotificationsList = _unitOfWork.Mapper.Map<List<LastNotificationsForAccountSpDto>>(result);

            return response;
        }


        [Route("Count"), HttpPost, Authorize]
        public async Task<GetNotificationsCountResponseDto> GetCountAsync(GetNotificationsCountRequestDto request)
        {
            AuthenticateUser();

            var response = new GetNotificationsCountResponseDto();

            var sql = $"SELECT COUNT(*) FROM Notifications WHERE {nameof(NotificationsEntity.RecipientId)} = @AccountId";
            response.TotalCount = await _unitOfWork.SqlConnection.QueryFirstAsync<int>(sql, new { _unitOfWork.AccountId });

            sql = $"SELECT COUNT(*) FROM Notifications WHERE {nameof(NotificationsEntity.RecipientId)} = @AccountId AND {nameof(NotificationsEntity.ReadDate)} IS NULL";
            response.UnreadCount = await _unitOfWork.SqlConnection.QueryFirstAsync<int>(sql, new { _unitOfWork.AccountId });

            return response;
        }



        [Route("Add"), HttpPost, Authorize]
        public async Task<ResponseDtoBase> AddAsync(AddNotificationRequestDto request)
        {
            AuthenticateUser();

            var response = new ResponseDtoBase();

            var sql = "SELECT TOP 1 Id FROM Accounts WHERE Id = @AccountId";
            var senderId = await _unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<int?>(sql, new { _unitOfWork.AccountId }) ?? throw new NotFoundException($"Пользователь-отправитель с Id {_unitOfWork.AccountId} не найден!");

            sql = "SELECT TOP 1 Id FROM Accounts WHERE Id = @RecipientId";
            var recipientId = await _unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<int?>(sql, new { request.RecipientId }) ?? throw new NotFoundException($"Пользователь-получатель с Id {_unitOfWork.AccountId} не найден!");

            sql = $"INSERT INTO Notifications ({nameof(NotificationsEntity.SenderId)}, {nameof(NotificationsEntity.RecipientId)}, {nameof(NotificationsEntity.Text)}) " +
                "VALUES (@senderId, @recipientId, @Text)";
            await _unitOfWork.SqlConnection.ExecuteAsync(sql, new { senderId, recipientId, request.Text });

            return response;
        }
    }
}
