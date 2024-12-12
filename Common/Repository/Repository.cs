using Common.Dto.Requests;
using Common.Dto.Responses;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Common.Repository
{
    public class Repository<TRequestDto, TResponseDto> : IRepository<TRequestDto, TResponseDto>
        where TRequestDto : RequestDtoBase
        where TResponseDto : ResponseDtoBase, new()
    {

        readonly IConfiguration _config;

        public Repository(IConfiguration config) =>
            _config = config;


        public async Task<ApiResponse<TResponseDto>> HttpPostAsync(TRequestDto request)
        {
            var apiResponse = await PostAsJsonAsync<TRequestDto, TResponseDto>(request);
            return apiResponse;
        }


        private async Task<ApiResponse<TResponse>> PostAsJsonAsync<TRequest, TResponse>(TRequest request) 
            where TRequest : RequestDtoBase
            where TResponse : ResponseDtoBase, new()
        {
            var apiResponse = new ApiResponse<TResponse>();

            try
            {
                var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = null;

                if (!string.IsNullOrWhiteSpace(request.Token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.Token);

                //var host = _config.GetRequiredSection("WebAPI:Host").Value;
                //var host = "http://10.0.2.2:7010/api";
                var host = "http://localhost:7010/api";

                var response = await client.PostAsJsonAsync($"{host}{request.Uri}", request);

                apiResponse.StatusCode = response.StatusCode;

                if (apiResponse.StatusCode != HttpStatusCode.Unauthorized)
                    apiResponse.Response = (await response.Content.ReadFromJsonAsync<TResponse>())!;
            }
            catch (Exception ex) 
            {
                apiResponse.StatusCode=HttpStatusCode.InternalServerError;
                apiResponse.Response.ErrorMessage = ex.Message;
            }

            return apiResponse;
        }
    }
}
