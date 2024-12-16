using AutoMapper;
using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Dapper;
using Data.Entities.Views;
using WebAPI.Exceptions;
using WebAPI.Models;

namespace WebAPI.Extensions
{
    public static partial class GetEventsRequestDtoExtension
    {
        public static async Task GetEventsAsync(this GetEventsRequestDto request, UnitOfWork unitOfWork, List<string> columns, GetEventsResponseDto response)
        {
            if (request.IsPhotosIncluded)
                columns.Add(nameof(EventsViewEntity.Photos));

            if (request.EventId != null)
            {
                var sql = $"SELECT {columns.Aggregate((a, b) => a + ", " + b)} " +
                    $"FROM EventsView " +
                    $"WHERE Id = @EventId";
                var result = await unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<EventsViewEntity>(sql, new { request.EventId })
                    ?? throw new NotFoundException("Мероприятие не найдено!");
                response.Event = unitOfWork.Mapper.Map<EventsViewDto>(result);
            }
        }
    }
}
