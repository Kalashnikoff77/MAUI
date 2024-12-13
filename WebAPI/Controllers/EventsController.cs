using AutoMapper;
using Common.Dto;
using Common.Dto.Requests;
using Common.Dto.Responses;
using Common.Dto.Views;
using Common.Models;
using Dapper;
using DataContext.Entities;
using DataContext.Entities.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebAPI.Exceptions;
using WebAPI.Extensions;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : MyControllerBase
    {
        public EventsController(IMapper mapper, IConfiguration configuration, IMemoryCache cache) : base(configuration, mapper, cache) { }


        [Route("Get"), HttpPost]
        public async Task<GetEventsResponseDto?> GetEventsAsync(GetEventsRequestDto request)
        {
            var response = new GetEventsResponseDto();

            var data = _unitOfWork.CacheTryGet(request, response, StaticData.CachePrefixEvents);
            if (data == null)
            {
                var columns = GetRequiredColumns<EventsViewEntity>();
                await request.GetEventsAsync(_unitOfWork, columns, response);
                _unitOfWork.CacheSet(request, response, StaticData.CachePrefixEvents);
            }
            else
                response = data;

            return response;
        }


        [Route("GetSchedules"), HttpPost]
        public async Task<GetSchedulesResponseDto?> GetSchedulesAsync(GetSchedulesRequestDto request)
        {
            var response = new GetSchedulesResponseDto();

            var data = _unitOfWork.CacheTryGet(request, response, StaticData.CachePrefixEvents);
            if (data == null)
            {
                var columns = GetRequiredColumns<SchedulesForEventsViewEntity>();

                // Получить одно расписание определённого мероприятия
                if (request.ScheduleId.HasValue && request.ScheduleId > 0)
                    await request.GetOneScheduleForEventAsync(_unitOfWork, columns, response);
                // Получить все расписания определённого мероприятия
                else if (request.EventId.HasValue && request.EventId > 0)
                    await request.GetAllSchedulesForEventAsync(_unitOfWork, columns, response);
                // Получить несколько расписаний разных мероприятий (по фильтрам) (кэшируется)
                else
                    await request.GetFilteredSchedulesForEventAsync(_unitOfWork, columns, response);

                _unitOfWork.CacheSet(request, response, StaticData.CachePrefixEvents);
            }
            else
                response = data;

            return response;
        }


        /// <summary>
        /// Получает список всех дат расписания указанного мероприятия
        /// </summary>
        [Route("GetSchedulesDates"), HttpPost]
        public async Task<GetSchedulesDatesResponseDto?> GetSchedulesDatesAsync(GetSchedulesDatesRequestDto request)
        {
            var response = new GetSchedulesDatesResponseDto();

            var data = _unitOfWork.CacheTryGet(request, response, StaticData.CachePrefixEvents);
            if (data == null)
            {
                await request.GetSchedulesDatesAsync(_unitOfWork, response);
                _unitOfWork.CacheSet(request, response, StaticData.CachePrefixEvents);
            }
            else
                response = data;

            return response;
        }


        [Route("GetDiscussions"), HttpPost]
        public async Task<GetDiscussionsForEventsResponseDto?> GetDiscussions(GetDiscussionsForEventsRequestDto request)
        {
            if (request.EventId == 0)
                throw new NotFoundException("Не указано мероприятие (EventId = 0)!");

            var response = new GetDiscussionsForEventsResponseDto();

            var data = _unitOfWork.CacheTryGet(request, response, StaticData.CachePrefixEvents);
            if (data == null)
            {
                IEnumerable<DiscussionsForEventsEntity> result;

                // Получим кол-во сообщений в чате мероприятия
                var sql = $"SELECT COUNT(*) FROM DiscussionsForEvents " +
                    $"WHERE {nameof(DiscussionsForEventsViewEntity.EventId)} = @{nameof(DiscussionsForEventsViewEntity.EventId)}";
                response.NumOfDiscussions = await _unitOfWork.SqlConnection.QuerySingleAsync<int>(sql, new { request.EventId });

                // Запрос на получение предыдущих сообщений
                if (request.GetPreviousFromId.HasValue)
                {
                    sql = $"SELECT TOP (@Take) * FROM DiscussionsForEventsView " +
                            $"WHERE {nameof(DiscussionsForEventsViewEntity.EventId)} = @{nameof(DiscussionsForEventsViewEntity.EventId)} " +
                            $"AND Id < {request.GetPreviousFromId} " +
                            $"ORDER BY Id DESC";
                    result = (await _unitOfWork.SqlConnection.QueryAsync<DiscussionsForEventsViewEntity>(sql, new { request.EventId, request.Take })).Reverse();
                }

                // Запрос на получение следующих сообщений
                else if (request.GetNextAfterId.HasValue)
                {
                    sql = $"SELECT TOP (@Take) * FROM DiscussionsForEventsView " +
                            $"WHERE {nameof(DiscussionsForEventsViewEntity.EventId)} = @{nameof(DiscussionsForEventsViewEntity.EventId)} " +
                            $"AND Id > {request.GetNextAfterId} " +
                            $"ORDER BY Id ASC";
                    result = await _unitOfWork.SqlConnection.QueryAsync<DiscussionsForEventsViewEntity>(sql, new { request.EventId, request.Take });
                }

                // Запрос на получение последних сообщений (по умолчанию)
                else
                {
                    sql = $"SELECT TOP (@Take) * FROM DiscussionsForEventsView " +
                        $"WHERE {nameof(DiscussionsForEventsViewEntity.EventId)} = @{nameof(DiscussionsForEventsViewEntity.EventId)} " +
                        $"ORDER BY Id DESC";
                    result = (await _unitOfWork.SqlConnection.QueryAsync<DiscussionsForEventsViewEntity>(sql, new { request.EventId, request.Take })).Reverse();
                }

                response.Discussions = _unitOfWork.Mapper.Map<List<DiscussionsForEventsViewDto>>(result);
                
                _unitOfWork.CacheSet(request, response, StaticData.CachePrefixEvents);
            }
            else
                response = data;

            return response;
        }


        [Route("AddDiscussion"), HttpPost, Authorize]
        public async Task<AddDiscussionsForEventsResponseDto?> AddDiscussion(AddDiscussionsForEventsRequestDto request)
        {
            if (request.EventId == 0)
                throw new NotFoundException("Не указано мероприятие (EventId = 0)!");

            if (string.IsNullOrWhiteSpace(request.Text))
                throw new BadRequestException("Вы не ввели текст сообщения!");

            AuthenticateUser();

            var sql = $"INSERT INTO DiscussionsForEvents " +
                $"({nameof(DiscussionsForEventsEntity.EventId)}, {nameof(DiscussionsForEventsEntity.SenderId)}, {nameof(DiscussionsForEventsEntity.RecipientId)}, {nameof(DiscussionsForEventsEntity.DiscussionId)}, {nameof(DiscussionsForEventsEntity.Text)}) " +
                $"OUTPUT INSERTED.Id " +
                $"VALUES " +
                $"(@{nameof(DiscussionsForEventsEntity.EventId)}, @AccountId, @{nameof(DiscussionsForEventsEntity.RecipientId)}, @{nameof(DiscussionsForEventsEntity.DiscussionId)}, @{nameof(DiscussionsForEventsEntity.Text)})";
            var newId = await _unitOfWork.SqlConnection.QuerySingleAsync<int>(sql, new { request.EventId, _unitOfWork.AccountId, request.RecipientId, request.DiscussionId, request.Text });

            _unitOfWork.CacheClear(StaticData.CachePrefixEvents);

            return new AddDiscussionsForEventsResponseDto { NewDiscussionId = newId };
        }


        /// <summary>
        /// Проверка при добавлении или обновлении мероприятия
        /// </summary>
        [Route("Check"), HttpPost, Authorize]
        public async Task<EventCheckResponseDto> CheckAsync(EventCheckRequestDto request)
        {
            var response = new EventCheckResponseDto();
           
            if (request.EventName != null)
            {
                string sql;
                if (request.EventId.HasValue)
                    sql = $"SELECT TOP 1 Id FROM Events WHERE Name = @EventName AND Id <> @EventId";
                else
                    sql = $"SELECT TOP 1 Id FROM Events WHERE Name = @EventName";
                var result = await _unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<int?>(sql, new { request.EventId, request.EventName });
                response.EventNameExists = result == null ? false : true;
            }
            return response;
        }


        /// <summary>
        /// Получает список пользователей, которые являются админами текущих мероприятий
        /// </summary>
        [Route("GetAdminsForEvents"), HttpPost]
        public async Task<GetAdminsForEventsResponseDto> GetAdminsForEventsAsync(GetAdminsForEventsRequestDto request)
        {
            var response = new GetAdminsForEventsResponseDto();

            var data = _unitOfWork.CacheTryGet(request, response, StaticData.CachePrefixEvents);
            if (data == null)
            {
                var sql = "SELECT * FROM AdminsForEventsView ORDER BY Name";
                var result = await _unitOfWork.SqlConnection.QueryAsync<AdminsForEventsViewEntity>(sql);
                response.AdminsForEvents = _unitOfWork.Mapper.Map<List<AdminsForEventsViewDto>>(result);

                _unitOfWork.CacheSet(request, response, StaticData.CachePrefixEvents);
            }
            else
                response = data;

            return response;
        }


        /// <summary>
        /// Получить список всех features
        /// </summary>
        [Route("GetFeatures"), HttpPost]
        public async Task<GetFeaturesResponseDto> GetFeaturesAsync(GetFeaturesRequestDto request)
        {
            var response = new GetFeaturesResponseDto();

            var data = _unitOfWork.CacheTryGet(request, response);
            if (data == null)
            {
                var sql = $"SELECT * FROM Features ORDER BY Name";
                var result = await _unitOfWork.SqlConnection.QueryAsync<FeaturesEntity>(sql);
                response.Features = _unitOfWork.Mapper.Map<List<FeaturesDto>>(result);
                _unitOfWork.CacheSet(request, response, expiration: 300);
            }
            else
                response = data;

            return response;
        }


        /// <summary>
        /// Получить список всех features, которые фигурируют в мероприятиях плюс кол-во мероприятий, в которых они фигурируют
        /// </summary>
        [Route("GetFeaturesForEvents"), HttpPost]
        public async Task<GetFeaturesForEventsResponseDto> GetFeaturesForEventsAsync(GetFeaturesForEventsRequestDto request)
        {
            var response = new GetFeaturesForEventsResponseDto();

            var data = _unitOfWork.CacheTryGet(request, response, StaticData.CachePrefixEvents);
            if (data == null)
            {
                var sql = $"SELECT * FROM FeaturesForEventsView";
                var result = await _unitOfWork.SqlConnection.QueryAsync<FeaturesForEventsViewEntity>(sql);
                response.FeaturesForEvents = _unitOfWork.Mapper.Map<List<FeaturesForEventsViewDto>>(result);
                _unitOfWork.CacheSet(request, response, StaticData.CachePrefixEvents);
            }
            else
                response = data;

            return response;
        }


        /// <summary>
        /// Получить список пользователей, зарегистрированных на определённое мероприятие
        /// </summary>
        [Route("GetSchedulesForAccounts"), HttpPost]
        public async Task<GetSchedulesForAccountsResponseDto> GetSchedulesForAccounts(GetSchedulesForAccountsRequestDto request)
        {
            var response = new GetSchedulesForAccountsResponseDto();

            var data = _unitOfWork.CacheTryGet(request, response, StaticData.CachePrefixEvents);
            if (data == null)
            {
                var sql = $"SELECT * FROM SchedulesForAccountsView " +
                    $"WHERE {nameof(SchedulesForAccountsEntity.ScheduleId)} = @{nameof(SchedulesForAccountsEntity.ScheduleId)} " +
                    $"ORDER BY {nameof(SchedulesForAccountsEntity.PurchaseDate)} DESC";
                var result = await _unitOfWork.SqlConnection.QueryAsync<SchedulesForAccountsViewEntity>(sql, new { request.ScheduleId });
                response.Accounts = _unitOfWork.Mapper.Map<List<SchedulesForAccountsViewDto>>(result);
                
                _unitOfWork.CacheSet(request, response, StaticData.CachePrefixEvents);
            }
            else
                response = data;

            return response;
        }


        /// <summary>
        /// Добавление мероприятия
        /// </summary>
        [Route("Add"), HttpPost, Authorize]
        public async Task<AddEventResponseDto> AddAsync(AddEventRequestDto request)
        {
            AuthenticateUser();

            var response = new AddEventResponseDto();

            // Проверка
            request.Validate(_unitOfWork);

            await _unitOfWork.BeginTransactionAsync();

            // Добавление мероприятия
            request.Event.Id = await request.AddEventAsync(_unitOfWork);

            //// Добавление расписаний мероприятия
            await request.AddSchedulesAsync(_unitOfWork);

            //// Добавление фото мероприятия
            await request.AddPhotosAsync(_unitOfWork);

            await _unitOfWork.CommitTransactionAsync();

            _unitOfWork.CacheClear(StaticData.CachePrefixEvents);

            response.NewEventId = request.Event.Id;
            return response;
        }


        /// <summary>
        /// Обновление мероприятия
        /// </summary>
        [Route("Update"), HttpPost, Authorize]
        public async Task<UpdateEventResponseDto> UpdateAsync(UpdateEventRequestDto request)
        {
            AuthenticateUser();

            // Проверка
            request.Validate(_unitOfWork);

            var response = new UpdateEventResponseDto();

            await _unitOfWork.BeginTransactionAsync();

            // Обновление мероприятия
            await request.UpdateEventAsync(_unitOfWork);

            // Обновление расписаний мероприятия
            await request.UpdateSchedulesAsync(_unitOfWork);

            // Обновление фото меропириятия
            await request.UpdatePhotosAsync(_unitOfWork);

            await _unitOfWork.CommitTransactionAsync();

            _unitOfWork.CacheClear(StaticData.CachePrefixEvents);

            return response;
        }
    }
}
