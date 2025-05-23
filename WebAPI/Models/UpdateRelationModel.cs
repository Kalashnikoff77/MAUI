﻿using Dapper;
using Data.Dto.Responses;
using Data.Entities;
using Data.Enums;
using System.Data.Common;

namespace WebAPI.Models
{
    public class UpdateRelationModel
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        public DbConnection Conn { get; set; } = null!;

        public UpdateRelationResponseDto Response { get; set; } = null!;

        /// <summary>
        /// Удаление всех связей
        /// </summary>
        public async Task RemoveAllRelationsAsync()
        {
            var sql = $"DELETE FROM RelationsForAccounts " +
            $"WHERE ({nameof(RelationsForAccountsEntity.SenderId)} = @{nameof(RelationsForAccountsEntity.SenderId)} AND {nameof(RelationsForAccountsEntity.RecipientId)} = @{nameof(RelationsForAccountsEntity.RecipientId)}) " +
                $"OR ({nameof(RelationsForAccountsEntity.SenderId)} = @{nameof(RelationsForAccountsEntity.RecipientId)} AND {nameof(RelationsForAccountsEntity.RecipientId)} = @{nameof(RelationsForAccountsEntity.SenderId)})";
            await Conn.ExecuteAsync(sql, new { SenderId, RecipientId });
        }

        
        /// <summary>
        /// Блокировка пользователя
        /// </summary>
        public async Task BlockAsync()
        {
            // Проверим, заблокирован ли пользователь в данный момент?
            var sql = $"SELECT TOP 1 Id FROM RelationsForAccounts WHERE " +
                $"(({nameof(RelationsForAccountsEntity.SenderId)} = @SenderId AND {nameof(RelationsForAccountsEntity.RecipientId)} = @RecipientId) " +
                $"OR " +
                $"({nameof(RelationsForAccountsEntity.SenderId)} = @RecipientId AND {nameof(RelationsForAccountsEntity.RecipientId)} = @SenderId)) " +
                $"AND Type = @Type";
            var currentBlocked = await Conn.QueryFirstOrDefaultAsync<int?>(sql, new { SenderId, RecipientId, Type = (short)EnumRelations.Blocked });

            // Удалим все связи обоих пользователей
            await RemoveAllRelationsAsync();

            // Удалим все уведомления (кроме уведомлений о блокировке) у обоих пользователей
            sql = $"UPDATE Messages SET {nameof(MessagesEntity.IsDeleted)} = 1 WHERE " +
                $"(({nameof(RelationsForAccountsEntity.SenderId)} = @SenderId AND {nameof(RelationsForAccountsEntity.RecipientId)} = @RecipientId) " +
                $"OR " +
                $"({nameof(RelationsForAccountsEntity.SenderId)} = @RecipientId AND {nameof(RelationsForAccountsEntity.RecipientId)} = @SenderId)) " +
                $"AND {nameof(MessagesEntity.Type)} NOT IN ({(short)EnumMessages.Message},{(short)EnumMessages.AccountBlocked},{(short)EnumMessages.AccountUnblocked})";
            await Conn.ExecuteAsync(sql, new { SenderId, RecipientId });

            // Добавим связь "Заблокирован"
            if (currentBlocked == null)
            {
                sql = $"INSERT INTO RelationsForAccounts ({nameof(RelationsForAccountsEntity.SenderId)}, {nameof(RelationsForAccountsEntity.RecipientId)}, {nameof(RelationsForAccountsEntity.Type)}, {nameof(RelationsForAccountsEntity.IsConfirmed)}) " +
                    $"VALUES " +
                    $"(@{nameof(RelationsForAccountsEntity.SenderId)}, @{nameof(RelationsForAccountsEntity.RecipientId)}, {(short)EnumRelations.Blocked}, 1)";
                await Conn.ExecuteAsync(sql, new { SenderId, RecipientId });
                Response.IsRelationAdded = true;
            }
            else
            {
                Response.IsRelationAdded = false;
            }
        }


        /// <summary>
        /// Дружба пользователя
        /// </summary>
        public async Task FriendshipAsync() 
        {
            // Проверим, есть ли такая связь?
            var sql = $"SELECT TOP 1 * FROM RelationsForAccounts WHERE " +
                $"(({nameof(RelationsForAccountsEntity.SenderId)} = @{nameof(RelationsForAccountsEntity.SenderId)} AND {nameof(RelationsForAccountsEntity.RecipientId)} = @{nameof(RelationsForAccountsEntity.RecipientId)}) " +
                $"OR ({nameof(RelationsForAccountsEntity.SenderId)} = @{nameof(RelationsForAccountsEntity.RecipientId)} AND {nameof(RelationsForAccountsEntity.RecipientId)} = @{nameof(RelationsForAccountsEntity.SenderId)})) " +
                $"AND {nameof(RelationsForAccountsEntity.Type)} = {(short)EnumRelations.Friend}";
            var currentRelation = await Conn.QueryFirstOrDefaultAsync<RelationsForAccountsEntity>(sql, new { SenderId, RecipientId });

            // Если связь не существует, то добавляем
            if (currentRelation == null)
            {
                sql = $"INSERT INTO RelationsForAccounts ({nameof(RelationsForAccountsEntity.SenderId)}, {nameof(RelationsForAccountsEntity.RecipientId)}, {nameof(RelationsForAccountsEntity.Type)}, {nameof(RelationsForAccountsEntity.IsConfirmed)}) " +
                    $"VALUES " +
                    $"(@{nameof(RelationsForAccountsEntity.SenderId)}, @{nameof(RelationsForAccountsEntity.RecipientId)}, {(short)EnumRelations.Friend}, 0)";
                await Conn.ExecuteAsync(sql, new { SenderId, RecipientId });
                Response.IsRelationAdded = true;
            }
            else
            {
                // Если связь подтверждена, то удаляем
                if (currentRelation.IsConfirmed)
                    sql = $"DELETE FROM RelationsForAccounts WHERE Id = @Id";
                // Иначе связь подтверждаем
                else
                    sql = $"UPDATE RelationsForAccounts SET IsConfirmed = 1 WHERE Id = @Id";
                await Conn.ExecuteAsync(sql, new { currentRelation.Id });
                Response.IsRelationAdded = false;
            }
        }
    }
}
