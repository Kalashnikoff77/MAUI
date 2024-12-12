namespace Common.Models
{
    public static class StaticData
    {
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
        public const short MESSAGES_PER_BLOCK = 10;


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

        public const string TempPhotosDir = "../UI/wwwroot/images/temp";
        public const string AccountsPhotosDir = "../UI/wwwroot/images/AccountsPhotos";
        public const string EventsPhotosDir = "../UI/wwwroot/images/EventsPhotos";

        // Префикс (метка) для кэширования запросов, связанных с мероприятиями
        public const string CachePrefixEvents = "events";


    }
}
