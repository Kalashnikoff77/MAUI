using Common.Dto;
using Common.Dto.Views;

namespace Common.Extensions
{
    public static class EventsExtension
    {
        /// <summary>
        /// Конвертация страны и региона в текст
        /// </summary>
        /// <param name="finish">HTML, добавляемый в конец строки</param>
        /// <returns>"Россия, Москва"</returns>
        public static string ToRegionString(this EventsViewDto evt) =>
            $"{evt.Country!.Name}, {evt.Country.Region.Name}";

        public static string ToShortDescription(this EventsViewDto evt)
        {
            string description;

            if (evt.Description.Length > 95)
                description = evt.Description.Substring(0, 95) + "...";
            else
                description = evt.Description;

            return description;
        }


        public static string ToDateClass(this SchedulesForEventsDto sch)
        {
            string dateClass = null!;

            if (sch.StartDate < DateTime.Now && sch.EndDate < DateTime.Now)
                dateClass = "red-text";
            else if (sch.StartDate < DateTime.Now)
                dateClass = "orange-text";
            else if (sch.StartDate < DateTime.Now.AddDays(3))
                dateClass = "green-text";
            else if (sch.StartDate < DateTime.Now.AddDays(7))
                dateClass = "blue-text";

            return dateClass;
        }

    }
}
