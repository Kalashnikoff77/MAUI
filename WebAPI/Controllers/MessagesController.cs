﻿using AutoMapper;
using Dapper;
using Data.Dto;
using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Sp;
using Data.Dto.Views;
using Data.Entities;
using Data.Entities.Sp;
using Data.Entities.Views;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebAPI.Exceptions;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : MyControllerBase
    {
        public MessagesController(IMapper mapper, IConfiguration configuration, IMemoryCache cache) : base(configuration, mapper, cache) { }

        // Получить одно или все сообщения
        [Route("Get"), HttpPost, Authorize]
        public async Task<GetMessagesResponseDto> GetAsync(GetMessagesRequestDto request)
        {
            AuthenticateUser();

            var response = new GetMessagesResponseDto();

            // Получить сообщение с указанным типом
            if (request.Type != null)
            {
                MessagesEntity? result;
                var sql = $"SELECT * FROM Messages " +
                    $"WHERE (({nameof(MessagesEntity.SenderId)} = @AccountId AND {nameof(MessagesEntity.RecipientId)} = @RecipientId) " +
                    $"OR ({nameof(MessagesEntity.SenderId)} = @RecipientId AND {nameof(MessagesEntity.RecipientId)} = @AccountId)) " +
                    $"AND Type = @Type AND IsDeleted = 0";
                result = await _unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<MessagesEntity>(sql, new { _unitOfWork.AccountId, request.RecipientId, request.Type });
                response.Message = _unitOfWork.Mapper.Map<MessagesDto>(result);
            }
            else
            {
                IEnumerable<MessagesEntity> result;

                // Получим кол-во сообщений
                var sql = $"SELECT COUNT(*) FROM Messages " +
                    $"WHERE (({nameof(MessagesEntity.SenderId)} = @AccountId AND {nameof(MessagesEntity.RecipientId)} = @RecipientId) " +
                    $"OR ({nameof(MessagesEntity.SenderId)} = @RecipientId AND {nameof(MessagesEntity.RecipientId)} = @AccountId)) " +
                    $"AND IsDeleted = 0";
                response.Count = await _unitOfWork.SqlConnection.QueryFirstAsync<int>(sql, new { _unitOfWork.AccountId, request.RecipientId });

                // Запрос на получение предыдущих сообщений
                if (request.GetPreviousFromId.HasValue)
                {
                    sql = $"SELECT TOP (@Take) * FROM Messages " +
                        $"WHERE (({nameof(MessagesEntity.SenderId)} = @AccountId AND {nameof(MessagesEntity.RecipientId)} = @RecipientId) " +
                        $"OR ({nameof(MessagesEntity.SenderId)} = @RecipientId AND {nameof(MessagesEntity.RecipientId)} = @AccountId)) " +
                        $"AND Id < {request.GetPreviousFromId} " +
                        $"AND IsDeleted = 0 " +
                        $"ORDER BY Id DESC";
                    result = (await _unitOfWork.SqlConnection.QueryAsync<MessagesEntity>(sql, new { _unitOfWork.AccountId, request.RecipientId, request.Take })).Reverse();
                }

                // Запрос на получение следующих сообщений
                else if (request.GetNextAfterId.HasValue)
                {
                    sql = $"SELECT TOP (@Take) * FROM Messages " +
                        $"WHERE (({nameof(MessagesEntity.SenderId)} = @AccountId AND {nameof(MessagesEntity.RecipientId)} = @RecipientId) " +
                        $"OR ({nameof(MessagesEntity.SenderId)} = @RecipientId AND {nameof(MessagesEntity.RecipientId)} = @AccountId)) " +
                        $"AND Id > {request.GetNextAfterId} " +
                        $"AND IsDeleted = 0 " +
                        $"ORDER BY Id ASC";
                    result = await _unitOfWork.SqlConnection.QueryAsync<MessagesEntity>(sql, new { _unitOfWork.AccountId, request.RecipientId, request.Take });
                }

                // Запрос на получение последних сообщений (по умолчанию)
                else
                {
                    int offset = response.Count.Value > StaticData.MESSAGES_PER_BLOCK ? response.Count.Value - StaticData.MESSAGES_PER_BLOCK : 0;
                    sql = $"SELECT * FROM Messages " +
                        $"WHERE (({nameof(MessagesEntity.SenderId)} = @AccountId AND {nameof(MessagesEntity.RecipientId)} = @RecipientId) " +
                        $"OR ({nameof(MessagesEntity.SenderId)} = @RecipientId AND {nameof(MessagesEntity.RecipientId)} = @AccountId)) " +
                        $"AND IsDeleted = 0 " +
                        $"ORDER BY Id ASC " +
                        $"OFFSET {offset} ROWS";
                    result = await _unitOfWork.SqlConnection.QueryAsync<MessagesEntity>(sql, new { _unitOfWork.AccountId, request.RecipientId });
                }

                response.Messages = _unitOfWork.Mapper.Map<List<MessagesDto>>(result);

                // Получим отправителя и получателя
                var columns = GetRequiredColumns<AccountsViewEntity>();
                sql = $"SELECT TOP 2 {columns.Aggregate((a, b) => a + ", " + b)} FROM AccountsView WHERE Id = @AccountId OR Id = @RecipientId";
                var accounts = await _unitOfWork.SqlConnection.QueryAsync<AccountsViewEntity>(sql, new { _unitOfWork.AccountId, request.RecipientId });

                response.Accounts = accounts.Select(x => new KeyValuePair<int, AccountsViewDto>(x.Id, _unitOfWork.Mapper.Map<AccountsViewDto>(x))).ToDictionary();
            }

            return response;
        }


        // Считает кол-во прочитанных и непрочитанных сообщений
        [Route("Count"), HttpPost, Authorize]
        public async Task<GetMessagesCountResponseDto> GetCountAsync(GetMessagesCountRequestDto request)
        {
            AuthenticateUser();

            var response = new GetMessagesCountResponseDto();

            var sql = $"SELECT COUNT(*) FROM Messages " +
                $"WHERE ({nameof(MessagesEntity.SenderId)} = @AccountId OR {nameof(MessagesEntity.RecipientId)} = @AccountId) AND {nameof(MessagesEntity.IsDeleted)} = 0";
            response.TotalCount = await _unitOfWork.SqlConnection.QueryFirstAsync<int>(sql, new { _unitOfWork.AccountId });

            sql = $"SELECT COUNT(*) FROM Messages " +
                $"WHERE ({nameof(MessagesEntity.RecipientId)} = @AccountId AND {nameof(MessagesEntity.ReadDate)} IS NULL) AND {nameof(MessagesEntity.IsDeleted)} = 0";
            response.UnreadCount = await _unitOfWork.SqlConnection.QueryFirstAsync<int>(sql, new { _unitOfWork.AccountId });

            return response;
        }


        [Route("MarkAsRead"), HttpPost, Authorize]
        public async Task<ResponseDtoBase> MarkAsReadAsync(MarkMessagesAsReadRequestDto request)
        {
            AuthenticateUser();

            // Помечаем все сообщения с этим пользователем как прочитанные
            if (request.MarkAllMessagesAsRead)
            {
                var sql = $"UPDATE Messages SET {nameof(MessagesEntity.ReadDate)} = getdate() " +
                    $"WHERE {nameof(MessagesEntity.RecipientId)} = @AccountId AND {nameof(MessagesEntity.SenderId)} = @SenderId " +
                    $"AND {nameof(MessagesEntity.ReadDate)} IS NULL";
                await _unitOfWork.SqlConnection.ExecuteAsync(sql, new { _unitOfWork.AccountId, request.SenderId });
            }
            
            // Помечаем одно или несколько сообщений с этим пользователем как прочитанные
            else
            {
                if (request.MessagesIds != null)
                {
                    var ids = request.MessagesIds.Select(s => s.ToString()).Aggregate((a, b) => a + "," + b);
                    var sql = $"UPDATE Messages SET {nameof(MessagesEntity.ReadDate)} = getdate() " +
                        $"WHERE Id IN ({ids}) AND " +
                        $"{nameof(MessagesEntity.RecipientId)} = @AccountId AND " +
                        $"{nameof(MessagesEntity.SenderId)} = @SenderId AND " +
                        $"{nameof(MessagesEntity.ReadDate)} IS NULL";
                    await _unitOfWork.SqlConnection.ExecuteAsync(sql, new { _unitOfWork.AccountId, request.SenderId });
                }
            }

            return new ResponseDtoBase();
        }


        [Route("GetLastMessagesList"), HttpPost, Authorize]
        public async Task<GetLastMessagesListResponseDto> GetLastMessagesListAsync(GetLastMessagesListRequestDto request)
        {
            AuthenticateUser();

            var response = new GetLastMessagesListResponseDto();

            var p = new DynamicParameters();
            p.Add("@AccountId", _unitOfWork.AccountId);
            var result = await _unitOfWork.SqlConnection.QueryAsync<LastMessagesForAccountSpEntity>("GetLastMessagesForAccount_sp", p, commandType: System.Data.CommandType.StoredProcedure);
            response.LastMessagesList = _unitOfWork.Mapper.Map<List<LastMessagesForAccountSpDto>>(result);

            return response;
        }


        [Route("Add"), HttpPost, Authorize]
        public async Task<AddMessageResponseDto> AddAsync(AddMessageRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                throw new BadRequestException("Вы не ввели текст сообщения!");

            AuthenticateUser();

            var response = new AddMessageResponseDto();

            var sql = "SELECT TOP 1 Id FROM Accounts WHERE Id = @AccountId";
            var senderId = await _unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<int?>(sql, new { _unitOfWork.AccountId }) ?? throw new NotFoundException($"Пользователь-отправитель с Id {_unitOfWork.AccountId} не найден!");

            sql = "SELECT TOP 1 Id FROM Accounts WHERE Id = @RecipientId";
            var recipientId = await _unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<int?>(sql, new { request.RecipientId }) ?? throw new NotFoundException($"Пользователь-получатель с Id {_unitOfWork.AccountId} не найден!");

            sql = $"INSERT INTO Messages ({nameof(MessagesEntity.Type)}, {nameof(MessagesEntity.SenderId)}, {nameof(MessagesEntity.RecipientId)}, {nameof(MessagesEntity.Text)}) " +
                $"OUTPUT INSERTED.Id " +
                $"VALUES (@Type, @senderId, @recipientId, @Text)";
            response.NewId = await _unitOfWork.SqlConnection.QuerySingleAsync<int>(sql, new { request.Type, senderId, recipientId, request.Text });

            return response;
        }


        [Route("Delete"), HttpPost, Authorize]
        public async Task<ResponseDtoBase> DeleteAsync(DeleteMessagesRequestDto request)
        {
            AuthenticateUser();

            var response = new ResponseDtoBase();

            var sql = "SELECT TOP 1 Id FROM Accounts WHERE Id = @AccountId";
            var senderId = await _unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<int?>(sql, new { _unitOfWork.AccountId }) ?? throw new NotFoundException($"Пользователь-отправитель с Id {_unitOfWork.AccountId} не найден!");

            sql = "SELECT TOP 1 Id FROM Accounts WHERE Id = @RecipientId";
            var recipientId = await _unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<int?>(sql, new { request.RecipientId }) ?? throw new NotFoundException($"Пользователь-получатель с Id {request.RecipientId} не найден!");

            if (request.DeleteAll)
            {
                sql = $"UPDATE Messages SET {nameof(MessagesEntity.IsDeleted)} = 1 " +
                    $"WHERE ({nameof(MessagesEntity.SenderId)} = @{nameof(MessagesEntity.SenderId)} AND {nameof(MessagesEntity.RecipientId)} = @{nameof(MessagesEntity.RecipientId)}) " +
                    $"OR ({nameof(MessagesEntity.SenderId)} = @{nameof(MessagesEntity.RecipientId)} AND {nameof(MessagesEntity.RecipientId)} = @{nameof(MessagesEntity.SenderId)})";
                await _unitOfWork.SqlConnection.ExecuteAsync(sql, new { SenderId = _unitOfWork.AccountId, request.RecipientId });
            }
            else
            {
                sql = $"UPDATE Messages SET {nameof(MessagesEntity.IsDeleted)} = 1 " +
                    $"WHERE Id = @MessageId AND " +
                    $"(({nameof(MessagesEntity.SenderId)} = @{nameof(MessagesEntity.SenderId)} AND {nameof(MessagesEntity.RecipientId)} = @{nameof(MessagesEntity.RecipientId)}) " +
                    $"OR ({nameof(MessagesEntity.SenderId)} = @{nameof(MessagesEntity.RecipientId)} AND {nameof(MessagesEntity.RecipientId)} = @{nameof(MessagesEntity.SenderId)}))";
                await _unitOfWork.SqlConnection.ExecuteAsync(sql, new { request.MessageId, SenderId = _unitOfWork.AccountId, request.RecipientId });
            }

            return response;
        }
    }
}
