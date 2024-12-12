using AutoMapper;
using Common.Dto.Requests;
using Common.Dto.Responses;
using Common.Dto.Views;
using Dapper;
using DataContext.Entities.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : MyControllerBase
    {
        public CountriesController(IMapper mapper, IConfiguration configuration, IMemoryCache cache) : base(configuration, mapper, cache) { }

        [Route("Get"), HttpPost]
        public async Task<GetCountriesResponseDto> GetAsync(GetCountriesRequestDto request)
        {
            var response = new GetCountriesResponseDto();

            var data = _unitOfWork.CacheTryGet(request, response);
            if (data == null)
            {
                IEnumerable<CountriesViewEntity> result;

                if (request.CountryId == null)
                    result = await _unitOfWork.SqlConnection.QueryAsync<CountriesViewEntity>($"SELECT * FROM CountriesView ORDER BY [Order] ASC, Name ASC");
                else
                    result = await _unitOfWork.SqlConnection.QueryAsync<CountriesViewEntity>($"SELECT TOP 1 * FROM CountriesView WHERE Id = @Id", new { Id = request.CountryId.Value });
                response.Countries = _unitOfWork.Mapper.Map<List<CountriesViewDto>>(result);
                _unitOfWork.CacheSet(request, response);
            }
            else
                response = data;

            return response;
        }


        /// <summary>
        /// Получает список регионов, в которых есть активные мероприятия
        /// </summary>
        [Route("GetRegionsForEvents"), HttpPost]
        public async Task<GetRegionsForEventsResponseDto> GetRegionsForEventsAsync(GetRegionsForEventsRequestDto request)
        {
            var response = new GetRegionsForEventsResponseDto();

            var data = _unitOfWork.CacheTryGet(request, response);
            if (data == null)
            {
                var sql = "SELECT * FROM RegionsForEventsView";
                var result = await _unitOfWork.SqlConnection.QueryAsync<RegionsForEventsViewEntity>(sql);
                response.RegionsForEvents = _unitOfWork.Mapper.Map<List<RegionsForEventsViewDto>>(result);
                _unitOfWork.CacheSet(request, response);
            }
            else
                response = data;

            return response;
        }
    }
}
