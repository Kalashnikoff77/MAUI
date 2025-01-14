using Data.Dto.Views;

namespace Shared.Components.Pages.Events
{
    public partial class Events
    {
        Filters Filters { get; set; } = new Filters();

        #region Поиск текста
        async Task OnSearch(string text)
        {
            request.FilterFreeText = text;
            SchedulesList.Clear();
            await LoadSchedulesAsync();
        }
        #endregion


        #region Фильтр услуг
        List<FeaturesForEventsViewDto> FeaturesList = new List<FeaturesForEventsViewDto>();

        async Task FeaturesChanged(IEnumerable<string> values)
        {
            Filters.SelectedFeatures = values;

            filteredAdmins.Clear();
            filteredRegions.Clear();

            request.FeaturesIds = FeaturesList
                .Where(x => values.Contains(x.Name))
                .Select(s => s.Id)
                .Distinct();

            SchedulesList.Clear();
            await LoadSchedulesAsync();
        }

        List<string> _filteredFeatures = new List<string>();
        List<string> filteredFeatures
        {
            get
            {
                if (_filteredFeatures.Count() > 0)
                    return _filteredFeatures;

                IEnumerable<FeaturesForEventsViewDto> linq = FeaturesList;

                if (isActualEvents)
                    linq = FeaturesList.Where(w => w.EndDate > DateTime.Now);
                else
                    linq = FeaturesList.Where(w => w.EndDate < DateTime.Now);

                if (request.RegionsIds?.Count() > 0)
                    linq = linq.Where(x => request.RegionsIds.Contains(x.RegionId));

                if (request.AdminsIds?.Count() > 0)
                    linq = linq.Where(x => request.AdminsIds.Contains(x.AdminId));

                _filteredFeatures = linq
                    .Select(s => s.Name)
                    .Distinct()
                    .OrderBy(o => o)
                    .ToList();

                return _filteredFeatures;
            }
        }
        #endregion


        #region Фильтр организаторов
        List<AdminsForEventsViewDto> AdminsList = new List<AdminsForEventsViewDto>();

        async Task AdminsChanged(IEnumerable<string> values)
        {
            Filters.SelectedAdmins = values;

            filteredFeatures.Clear();
            filteredRegions.Clear();

            request.AdminsIds = AdminsList
                .Where(x => values.Contains(x.Name))
                .Select(s => s.Id)
                .Distinct();

            SchedulesList.Clear();
            await LoadSchedulesAsync();
        }

        List<string> _filteredAdmins = new List<string>();
        List<string> filteredAdmins
        {
            get
            {
                if (_filteredAdmins.Count() > 0)
                    return _filteredAdmins;

                IEnumerable<AdminsForEventsViewDto> linq = AdminsList;

                if (isActualEvents)
                    linq = AdminsList.Where(w => w.EndDate > DateTime.Now);
                else
                    linq = AdminsList.Where(w => w.EndDate < DateTime.Now);

                if (request.RegionsIds?.Count() > 0)
                    linq = linq.Where(x => request.RegionsIds.Contains(x.RegionId));

                if (request.FeaturesIds?.Count() > 0)
                    linq = linq.Where(x => request.FeaturesIds.Contains(x.FeatureId));

                _filteredAdmins = linq
                    .Select(s => s.Name)
                    .Distinct()
                    .OrderBy(o => o)
                    .ToList();

                return _filteredAdmins;
            }
        }
        #endregion


        #region Фильтр регионов
        List<RegionsForEventsViewDto> RegionsList = new List<RegionsForEventsViewDto>();

        async Task RegionsChanged(IEnumerable<string> values)
        {
            Filters.SelectedRegions = values;

            filteredFeatures.Clear();
            filteredAdmins.Clear();

            request.RegionsIds = RegionsList
                .Where(x => values.Contains(x.Name))
                .Select(s => s.Id)
                .Distinct();

            SchedulesList.Clear();
            await LoadSchedulesAsync();
        }

        List<string> _filteredRegions = new List<string>();
        List<string> filteredRegions
        {
            get
            {
                if (_filteredRegions.Count() > 0)
                    return _filteredRegions;

                IEnumerable<RegionsForEventsViewDto> linq = RegionsList;

                if (isActualEvents)
                    linq = RegionsList.Where(w => w.EndDate > DateTime.Now);
                else
                    linq = RegionsList.Where(w => w.EndDate < DateTime.Now);

                if (request.AdminsIds?.Count() > 0)
                    linq = linq.Where(x => request.AdminsIds.Contains(x.AdminId));

                if (request.FeaturesIds?.Count() > 0)
                    linq = linq.Where(x => request.FeaturesIds.Contains(x.FeatureId));

                _filteredRegions = linq
                    .OrderBy(o => o.Order)
                    .Select(s => s.Name)
                    .Distinct()
                    .ToList();

                return _filteredRegions;
            }
        }
        #endregion


        #region Фильтр актуальных мероприятий
        string actualEventsLabel = "Актуальные мероприятия";
        bool isActualEvents = true;

        async Task ActualEventsChanged(bool value)
        {
            isActualEvents = value;

            request.IsActualEvents = value;
            actualEventsLabel = value ? "Актуальные мероприятия" : "Завершённые мероприятия";

            filteredFeatures.Clear();
            Filters.SelectedFeatures = null;
            request.FeaturesIds = null;

            filteredAdmins.Clear();
            Filters.SelectedAdmins = null;
            request.AdminsIds = null;

            filteredRegions.Clear();
            Filters.SelectedRegions = null;
            request.RegionsIds = null;

            SchedulesList.Clear();
            await LoadSchedulesAsync();
        }
        #endregion
    }


    class Filters
    {
        public IEnumerable<string>? SelectedFeatures { get; set; }
        public IEnumerable<string>? SelectedAdmins { get; set; }
        public IEnumerable<string>? SelectedRegions { get; set; }
    }
}
