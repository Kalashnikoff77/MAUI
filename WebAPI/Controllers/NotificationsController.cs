using AutoMapper;
using Common.Dto.Requests;
using Common.Dto.Responses;
using Common.Dto.Views;
using Dapper;
using DataContext.Entities;
using DataContext.Entities.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebAPI.Exceptions;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : MyControllerBase
    {
        public NotificationsController(IMapper mapper, IConfiguration configuration, IMemoryCache cache) : base(configuration, mapper, cache) { }

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


        [Route("Get"), HttpPost, Authorize]
        public async Task<GetNotificationsResponseDto?> GetAsync(GetNotificationsRequestDto request)
        {
            AuthenticateUser();

            var response = new GetNotificationsResponseDto();

            // Получим все уведомления (с фильтром)
            var sql = "SELECT * FROM NotificationsView " +
                $"WHERE {nameof(NotificationsViewEntity.RecipientId)} = @AccountId" +
                $"ORDER BY {nameof(NotificationsViewEntity.CreateDate)} DESC " +
                $"OFFSET {request.Skip} ROWS FETCH NEXT {request.Take} ROWS ONLY";
            var notifications = await _unitOfWork.SqlConnection.QueryAsync<NotificationsViewEntity>(sql, new { _unitOfWork.AccountId });
            response.Notifications = _unitOfWork.Mapper.Map<List<NotificationsViewDto>>(notifications);

            // Подсчитаем кол-во уведомлений (с фильтром)
            sql = "SELECT COUNT(*) FROM NotificationsView " +
                $"WHERE {nameof(NotificationsViewEntity.RecipientId)} = @AccountId";
            response.Count = await _unitOfWork.SqlConnection.QuerySingleAsync<int>(sql, new { _unitOfWork.AccountId });

            // Будем отмечать уведомления, как прочитанные?
            if (request.MarkAsRead && response.Notifications.Any(x => x.ReadDate == null))
            {
                var ids = response.Notifications
                    .Where(w => w.ReadDate == null)
                    .Select(s => s.Id.ToString());

                if (ids.Any())
                    await _unitOfWork.SqlConnection.ExecuteAsync($"UPDATE Notifications SET {nameof(NotificationsEntity.ReadDate)} = getdate() " +
                        $"WHERE Id IN ({ids.Aggregate((a, b) => a + ", " + b)})");
            }

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
