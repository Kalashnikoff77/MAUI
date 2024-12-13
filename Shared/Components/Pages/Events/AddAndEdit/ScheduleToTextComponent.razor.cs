using Microsoft.AspNetCore.Components;
using System.Text;

namespace Shared.Components.Pages.Events.AddAndEdit
{
    public partial class ScheduleToTextComponent
    {
        [Parameter, EditorRequired] public bool IsOneTimeEvent { get; set; }
        [Parameter, EditorRequired] public DateTime? StartDate { get; set; }
        [Parameter, EditorRequired] public DateTime? EndDate { get; set; }
        [Parameter, EditorRequired] public TimeSpan? StartTime { get; set; }
        [Parameter, EditorRequired] public TimeSpan? EndTime { get; set; }
        [Parameter] public HashSet<short> DaysOfWeek { get; set; } = null!;

        StringBuilder result { get; set; } = new StringBuilder(100);

        protected override void OnParametersSet()
        {
            result.Clear();

            if (IsOneTimeEvent)
            {
                if (StartDate.HasValue)
                {
                    result.Append($"Начало {StartDate.Value.ToString("dd.MM.yyyy")}");
                    if (StartTime.HasValue)
                        result.Append($" в {string.Format("{0:D2}:{1:D2}", StartTime.Value.Hours, StartTime.Value.Minutes)}");
                }

                if (EndDate.HasValue)
                {
                    result.Append($", завершение {EndDate.Value.ToString("dd.MM.yyyy")}");
                    if (EndTime.HasValue)
                        result.Append($" в {string.Format("{0:D2}:{1:D2}", EndTime.Value.Hours, EndTime.Value.Minutes)}");
                }
            }
            else
            {
                if (StartDate.HasValue)
                    result.Append($"Период с {StartDate.Value.ToString("dd.MM.yyyy")}");

                if (EndDate.HasValue)
                    result.Append($" по {EndDate.Value.ToString("dd.MM.yyyy")}");

                if (DaysOfWeek != null && DaysOfWeek.Count > 0)
                {
                    result.Append(" по ");

                    if (DaysOfWeek.Contains(1)) result.Append("Пн, ");
                    if (DaysOfWeek.Contains(2)) result.Append("Вт, ");
                    if (DaysOfWeek.Contains(3)) result.Append("Ср, ");
                    if (DaysOfWeek.Contains(4)) result.Append("Чт, ");
                    if (DaysOfWeek.Contains(5)) result.Append("Пт, ");
                    if (DaysOfWeek.Contains(6)) result.Append("Сб, ");
                    if (DaysOfWeek.Contains(0)) result.Append("Вс, ");

                    result.Remove(result.Length - 2, 2);
                }

                if (StartTime.HasValue)
                    result.Append($" с {string.Format("{0:D2}:{1:D2}", StartTime.Value.Hours, StartTime.Value.Minutes)}");

                if (EndTime.HasValue)
                    result.Append($" до {string.Format("{0:D2}:{1:D2}", EndTime.Value.Hours, EndTime.Value.Minutes)}");
            }

        }
    }
}
