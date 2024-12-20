using AutoMapper;
using Data.Dto;
using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Models;
using Dapper;
using Data.Entities;
using Data.Entities.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PhotoSauce.MagicScaler;
using WebAPI.Exceptions;
using Data.Enums;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotosController : MyControllerBase
    {
        public PhotosController(IMapper mapper, IConfiguration configuration, IMemoryCache cache) : base(configuration, mapper, cache) { }

        [Route("Get"), HttpPost, Authorize]
        public async Task<GetPhotosForAccountsResponseDto> GetAsync(GetPhotosForAccountRequestDto request)
        {
            AuthenticateUser();

            var response = new GetPhotosForAccountsResponseDto();

            var columns = GetRequiredColumns<PhotosForAccountsViewEntity>();

            // Получаем фото текущего пользователя или указанного в request?
            var accountId = request.AccountId == null ? _unitOfWork.AccountId : request.AccountId;

            var sql = "SELECT TOP (@Take) " +
                $"{columns.Aggregate((a, b) => a + ", " + b)} " +
                $"FROM AccountsPhotosView WHERE {nameof(PhotosForAccountsViewEntity.RelatedId)} = @accountId " +
                "ORDER BY Id DESC";
            var result = await _unitOfWork.SqlConnection.QueryAsync<PhotosForAccountsViewEntity>(sql, new { accountId, request.Take });
            response.Photos = _unitOfWork.Mapper.Map<List<PhotosForAccountsViewDto>>(result);

            return response;
        }


        [Route("UpdatePhotoForAccount"), HttpPost, Authorize]
        public async Task<ResponseDtoBase> UpdatePhotoForAccountAsync(UpdatePhotoForAccountRequestDto request)
        {
            AuthenticateUser();

            var response = new ResponseDtoBase();

            var sql = "SELECT TOP 1 * FROM PhotosForAccounts WHERE AccountId = @AccountId AND Guid = @Guid";
            var photo = await _unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<PhotosForAccountsEntity>(sql, new { _unitOfWork.AccountId, request.Guid }) 
                ?? throw new NotFoundException("Соответствующее фото не найдено!");

            // Смена аватара
            if (request.IsAvatarChanging)
            {
                if (photo.IsAvatar)
                {
                    sql = $"UPDATE PhotosForAccounts SET {nameof(PhotosForAccountsEntity.IsAvatar)} = 0 WHERE {nameof(PhotosForAccountsEntity.Id)} = @Id";
                    await _unitOfWork.SqlConnection.ExecuteAsync(sql, new { photo.Id });
                }
                else
                {
                    sql = $"UPDATE PhotosForAccounts SET {nameof(PhotosForAccountsEntity.IsAvatar)} = 0 WHERE {nameof(PhotosForAccountsEntity.RelatedId)} = @AccountId";
                    await _unitOfWork.SqlConnection.ExecuteAsync(sql, new { _unitOfWork.AccountId });
                    sql = $"UPDATE PhotosForAccounts SET {nameof(PhotosForAccountsEntity.IsAvatar)} = 1 WHERE {nameof(PhotosForAccountsEntity.Id)} = @Id";
                    await _unitOfWork.SqlConnection.ExecuteAsync(sql, new { photo.Id });
                }
            }

            // Смена комментария
            if (request.IsCommentChanging)
            {
                sql = $"UPDATE PhotosForAccounts SET {nameof(PhotosForAccountsEntity.Comment)} = @Comment WHERE {nameof(PhotosForAccountsEntity.Id)} = @Id";
                await _unitOfWork.SqlConnection.ExecuteAsync(sql, new { request.Comment, photo.Id });
            }

            // Удаление фото
            if (request.IsDeleting)
            {
                sql = $"UPDATE PhotosForAccounts SET {nameof(PhotosForAccountsEntity.IsDeleted)} = 1 WHERE {nameof(PhotosForAccountsEntity.Id)} = @Id";
                await _unitOfWork.SqlConnection.ExecuteAsync(sql, new { photo.Id });
            }

            return response;
        }


        /// <summary>
        /// Загрузка фото во временный каталог temp: оригинал и аватар 250х250
        /// </summary>
        [Route("UploadPhotoToTemp"), HttpPost]
        public async Task<UploadPhotoToTempResponseDto> UploadPhotoToTempAsync(UploadPhotoToTempRequestDto request)
        {
            AuthenticateUser();

            // Если EventId или AccountId = 0, то это новые создаваемые записи, проверять наличие их в БД нет смысли
            if (request.EventId.HasValue && request.EventId > 0)
            {
                var sql = $"SELECT TOP 1 Id FROM Events WHERE Id = @EventId AND {nameof(EventsEntity.AdminId)} = @{nameof(EventsEntity.AdminId)}";
                var evtId = await _unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<int?>(sql, new { request.EventId, AdminId = _unitOfWork.AccountId })
                    ?? throw new NotFoundException($"Мероприятие с Id {request.EventId} не найдено!");
            }
            if (request.AccountId.HasValue && request.AccountId > 0)
            {
                var sql = $"SELECT TOP 1 Id FROM Accounts WHERE Id = @AccountId";
                var accId = await _unitOfWork.SqlConnection.QueryFirstOrDefaultAsync<int?>(sql, new { _unitOfWork.AccountId })
                    ?? throw new NotFoundException($"Пользователь с Id {request.AccountId} не найден!");
            }

            var response = new UploadPhotoToTempResponseDto();

            if (request.File != null)
            {
                var guid = Guid.NewGuid();
                var tempDir = $"{StaticData.TempPhotosDir}/{guid}";

                Directory.CreateDirectory(tempDir);

                // Сохраняем оригинал файла
                using (MemoryStream output = new MemoryStream(request.File))
                    await System.IO.File.WriteAllBytesAsync($"{tempDir}/original.jpg", output.ToArray());

                // Сохраняем временный аватар файла 250x250
                using (MemoryStream output = new MemoryStream(50000))
                {
                    MagicImageProcessor.ProcessImage(new MemoryStream(request.File), output, StaticData.Images[EnumImageSize.s250x250]);
                    await System.IO.File.WriteAllBytesAsync($"{tempDir}/{EnumImageSize.s250x250}.jpg", output.ToArray());
                }

                if (request.AccountId.HasValue)
                    response.NewAccountPhoto = new PhotosForAccountsDto { Guid = guid, CreateDate = DateTime.Now };
                else
                    response.NewEventPhoto = new PhotosForEventsDto { Guid = guid, CreateDate = DateTime.Now };
            }

            return response;
        }
    }
}
