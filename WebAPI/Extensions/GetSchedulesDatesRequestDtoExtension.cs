using Common.Dto.Requests;
using Common.Dto.Responses;
using Common.Dto.Views;
using Dapper;
using DataContext.Entities.Views;
using WebAPI.Models;

namespace WebAPI.Extensions
{
    public static partial class GetSchedulesDatesRequestDtoExtension
    {
        public static async Task GetSchedulesDatesAsync(this GetSchedulesDatesRequestDto request, UnitOfWork unitOfWork, GetSchedulesDatesResponseDto response)
        {
            string sql = $"SELECT Id, {nameof(SchedulesDatesViewEntity.StartDate)}, {nameof(SchedulesDatesViewEntity.EndDate)} " +
            $"FROM SchedulesDatesView " +
            $"WHERE {nameof(SchedulesDatesViewEntity.EventId)} = @{nameof(SchedulesDatesViewEntity.EventId)} " +
            $"ORDER BY {nameof(SchedulesDatesViewEntity.StartDate)} ASC";
            var result = await unitOfWork.SqlConnection.QueryAsync<SchedulesDatesViewEntity>(sql, new { request.EventId });
            
            response.SchedulesDates = unitOfWork.Mapper.Map<List<SchedulesDatesViewDto>>(result);
        }
    }
}
