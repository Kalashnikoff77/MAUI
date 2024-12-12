using Common.Dto.Requests;
using Common.Dto.Responses;
using Common.Dto.Views;
using Dapper;
using DataContext.Entities.Views;
using System.Text.Json;
using WebAPI.Exceptions;
using WebAPI.Models;

namespace WebAPI.Extensions
{
    public static partial class GetSchedulesRequestDtoExtension
    {
        public static async Task GetOneScheduleForEventAsync(this GetSchedulesRequestDto request, UnitOfWork unitOfWork, List<string> columns, GetSchedulesResponseDto response)
        {
            var sql = $"SELECT {columns.Aggregate((a, b) => a + ", " + b)} " +
                $"FROM SchedulesForEventsView " +
                $"WHERE Id = @ScheduleId";
            var result = await unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<SchedulesForEventsViewEntity>(sql, new { request.ScheduleId })
                ?? throw new NotFoundException("Мероприятие не найдено!");

            response.Schedule = unitOfWork.Mapper.Map<SchedulesForEventsViewDto>(result);
        }

        public static async Task GetAllSchedulesForEventAsync(this GetSchedulesRequestDto request, UnitOfWork unitOfWork, List<string> columns, GetSchedulesResponseDto response)
        {
            var sql = $"SELECT {columns.Aggregate((a, b) => a + ", " + b)} " +
                $"FROM SchedulesForEventsView " +
                $"WHERE EventId = @EventId";
            var result = await unitOfWork.SqlConnection.QueryAsync<SchedulesForEventsViewEntity>(sql, new { request.EventId });

            response.Schedules = unitOfWork.Mapper.Map<List<SchedulesForEventsViewDto>>(result);
        }

        public static async Task GetFilteredSchedulesForEventAsync(this GetSchedulesRequestDto request, UnitOfWork unitOfWork, List<string> columns, GetSchedulesResponseDto response)
        {
            var jsonRequest = JsonSerializer.Serialize(request);
            // Сперва получим Id записей, которые нужно вытянуть + кол-во этих записей.
            var p = new DynamicParameters();
            p.Add("@GetEventsRequestDto", jsonRequest);
            var ids = await unitOfWork.SqlConnection.QueryAsync<int>("EventsFilter_sp", p, commandType: System.Data.CommandType.StoredProcedure);
            response.Count = ids.Count();

            if (response.Count > 0)
            {
                string order = request.IsActualEvents ? $"ORDER BY {nameof(SchedulesForEventsViewDto.StartDate)} " : $"ORDER BY {nameof(SchedulesForEventsViewDto.EndDate)} DESC ";

                string sql = $"SELECT {columns.Aggregate((a, b) => a + ", " + b)} " +
                    $"FROM SchedulesForEventsView " +
                    $"WHERE Id IN ({string.Join(",", ids)}) " +
                    order +
                    $"OFFSET {request.Skip} ROWS FETCH NEXT {request.Take} ROWS ONLY";

                var result = await unitOfWork.SqlConnection.QueryAsync<SchedulesForEventsViewEntity>(sql, new { request.EventId });
                response.Schedules = unitOfWork.Mapper.Map<List<SchedulesForEventsViewDto>>(result);
            }
        }
    }
}
