using Data.Dto.Requests;
using Data.Dto.Responses;

namespace Data.Services
{
    public interface IRepository<TRequestDto, TResponseDto>
        where TRequestDto : RequestDtoBase
        where TResponseDto : ResponseDtoBase, new()
    {
        Task<ApiResponse<TResponseDto>> HttpPostAsync(TRequestDto request);
    }
}
