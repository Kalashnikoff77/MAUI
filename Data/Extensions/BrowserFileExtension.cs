using Data.Dto;
using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace Data.Extensions
{
    public static class BrowserFileExtension
    {
        /// <summary>
        /// Сохранение фото во временном каталоге
        /// </summary>
        public static async Task<T?> Upload<T>(this IBrowserFile photo, string? token, IRepository<UploadPhotoToTempRequestDto, UploadPhotoToTempResponseDto> repoUploadPhotoToTemp, int? accountId = null, int? eventId = null)
            where T : PhotosDtoBase
        {
            using (var ms = new MemoryStream((int)photo.Size))
            {
                await photo.OpenReadStream(photo.Size).CopyToAsync(ms);

                var request = new UploadPhotoToTempRequestDto
                {
                    AccountId = accountId,
                    EventId = eventId,
                    Token = token,
                    File = ms.ToArray()
                };
                var apiResponse = await repoUploadPhotoToTemp.HttpPostAsync(request);

                if (accountId.HasValue)
                    return apiResponse.Response.NewAccountPhoto as T;
                else
                    return apiResponse.Response.NewEventPhoto as T;
            }
        }
    }
}
