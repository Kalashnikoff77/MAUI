using Common.Dto.Requests;
using Common.Dto.Responses;

namespace Common.Repository
{
    public interface IRepository<TRequestDto, TResponseDto> 
        where TRequestDto : RequestDtoBase
        where TResponseDto : ResponseDtoBase, new()
    {
        Task<ApiResponse<TResponseDto>> HttpPostAsync(TRequestDto request);
    }
}
