using Common.Extensions;

namespace Common.Dto.Views
{
    public class SchedulesDatesViewDto : DtoBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        /// <summary>
        /// Используется в Tab_About.razor для корректного вывода даты и времени
        /// </summary>
        public override string ToString() => StartDate.ToMyString();
    }
}
