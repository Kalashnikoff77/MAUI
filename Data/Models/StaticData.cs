using Data.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PhotoSauce.MagicScaler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Data.Models
{
    public static class StaticData
    {
        // CropScaleMode.Max - размер фото не увеличивать (фото может быть меньше указанных размеров). При необходимости урезать высоту или ширину
        // CropScaleMode.Crop - размер фото увеличить до указанных размеров. При необходимости урезать высоту или ширину

        /// <summary>
        /// Настройки размеров изображений
        /// </summary>
        public static Dictionary<EnumImageSize, ProcessImageSettings> Images = new()
        {
            {
                EnumImageSize.s64x64,
                new ProcessImageSettings { Width=64, Height=64, OrientationMode = OrientationMode.Normalize, ResizeMode=CropScaleMode.Crop }
            },
            {
                EnumImageSize.s150x150,
                new ProcessImageSettings { Width=150, Height=150, OrientationMode = OrientationMode.Normalize, ResizeMode=CropScaleMode.Crop }
            },
            {
                EnumImageSize.s250x250,
                new ProcessImageSettings { Width=250, Height=250, OrientationMode = OrientationMode.Normalize, ResizeMode=CropScaleMode.Crop }
            },
            {
                EnumImageSize.s450x600,
                new ProcessImageSettings { Width=450, Height=600, ResizeMode=CropScaleMode.Pad, OrientationMode = OrientationMode.Normalize, MatteColor = System.Drawing.Color.Black }
            },
            {
                EnumImageSize.s768x1024,
                new ProcessImageSettings { Width=768, Height=1024, ResizeMode=CropScaleMode.Pad, OrientationMode = OrientationMode.Normalize, MatteColor = System.Drawing.Color.Black }
            }
        };

        /// <summary>
        /// Пол
        /// </summary>
        public static Dictionary<int, Gender> Genders = new()
        {
            { 0, new Gender { ShortName = "М", Name = "Муж" } },
            { 1, new Gender { ShortName = "Ж", Name = "Жен" } }
        };
        public class Gender
        {
            public string ShortName { get; set; } = null!;
            public string Name { get; set; } = null!;
        }


        /// <summary>
        /// Кол-во сообщений в обсуждениях мероприятий в блоке
        /// </summary>
        public const short EVENT_DISCUSSIONS_PER_BLOCK = 10;

        /// <summary>
        /// Кол-во сообщений в чате
        /// </summary>
        public const short MESSAGES_PER_BLOCK = 20;

        /// <summary>
        /// Кол-во сообщений в блоке в разделе мероприятий - /events
        /// </summary>
        public const short SCHEDULES_PER_BLOCK = 10;


        // Константы полей БД
        public const short DB_ACCOUNTS_EMAIL_MIN = 5;
        public const short DB_ACCOUNTS_EMAIL_MAX = 75;
        public const short DB_ACCOUNTS_PASSWORD_MIN = 4;
        public const short DB_ACCOUNTS_PASSWORD_MAX = 35;
        public const short DB_ACCOUNTS_NAME_MIN = 3;
        public const short DB_ACCOUNTS_NAME_MAX = 25;
        public const short DB_USERS_NAME_MIN = 3;
        public const short DB_USERS_NAME_MAX = 25;
        public const short DB_USERS_ABOUT_MAX = 255;
        public const short DB_USERS_WEIGHT_MIN = 40;
        public const short DB_USERS_WEIGHT_MAX = 200;
        public const short DB_USERS_HEIGHT_MIN = 100;
        public const short DB_USERS_HEIGHT_MAX = 230;
        public const short DB_EVENT_NAME_MIN = 10;
        public const short DB_EVENT_NAME_MAX = 80;
        public const short DB_EVENT_DESCRIPTION_MIN = 50;
        public const short DB_EVENT_DESCRIPTION_MAX = 2048;
        public const short DB_EVENT_ADDRESS_MIN = 10;
        public const short DB_EVENT_ADDRESS_MAX = 150;

        public const string TempPhotosDir = "../Web/wwwroot/images/temp";
        public const string AccountsPhotosDir = "../Web/wwwroot/images/AccountsPhotos";
        public const string EventsPhotosDir = "../Web/wwwroot/images/EventsPhotos";

        // Префикс (метка) для кэширования запросов, связанных с мероприятиями
        public const string CachePrefixEvents = "events";



        /// <summary>
        /// Генерация токена JWT
        /// </summary>
        /// <param name="Id">Id пользователя</param>
        /// <param name="Guid">Guid пользователя</param>
        /// <param name="configuration">Ссылка на IConfiguration</param>
        /// <returns>JWT токен</returns>
        public static string GenerateToken(int Id, Guid Guid, IConfiguration configuration)
        {
            var claims = new List<Claim>()
            {
                new Claim("Id", Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim("Guid", Guid.ToString()),
                new Claim("IssueDate", DateTime.Now.ToString())
            };
            var jwt = new JwtSecurityToken(
                issuer: configuration.GetRequiredSection("JWT:JwtValidIssuer").Value,
                audience: configuration.GetRequiredSection("JWT:JwtValidAudience").Value,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(180)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetRequiredSection("JWT:IssuerSigningKey").Value!)), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
