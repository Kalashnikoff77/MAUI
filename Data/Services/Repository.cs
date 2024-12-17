using Data.Dto.Requests;
using Data.Dto.Responses;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Data.Services
{
    public class Repository<TRequestDto, TResponseDto> : IRepository<TRequestDto, TResponseDto>
        where TRequestDto : RequestDtoBase
        where TResponseDto : ResponseDtoBase, new()
    {

        readonly IConfiguration _config;
        readonly IFormFactor _formFactor;

        public Repository(IConfiguration config, IFormFactor formFactor) 
        {
            _config = config;
            _formFactor = formFactor;
        }


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

                string? host;
                if (_formFactor.GetFormFactor() == "Phone")
                    host = _config.GetRequiredSection("WebAPI:AndroidHost").Value;
                else
                    host = _config.GetRequiredSection("WebAPI:WinHost").Value;

                var response = await client.PostAsJsonAsync($"{host}{request.Uri}", request);

                apiResponse.StatusCode = response.StatusCode;

                if (apiResponse.StatusCode != HttpStatusCode.Unauthorized)
                    apiResponse.Response = (await response.Content.ReadFromJsonAsync<TResponse>())!;
            }
            catch (Exception ex)
            {
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.Response.ErrorMessage = ex.Message;
            }

            return apiResponse;
        }
    }
}
