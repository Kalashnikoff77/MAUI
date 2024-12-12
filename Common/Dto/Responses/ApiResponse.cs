using System.Net;

namespace Common.Dto.Responses
{
    public class ApiResponse<TResponseDto> where TResponseDto : ResponseDtoBase, new()
    {
        public HttpStatusCode StatusCode { get; set; }

        public TResponseDto Response { get; set; } = new TResponseDto();
    }
}
