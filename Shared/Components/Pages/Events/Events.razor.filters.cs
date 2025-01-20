using Data.Dto.Views;

namespace Shared.Components.Pages.Events
{
    public partial class Events
    {
        Filters Filters { get; set; } = new Filters();

        #region Поиск текста
        async Task OnSearch(string text)
        {
            _request.FilterFreeText = text;
            await ReloadItemsAsync();
        }
        #endregion


        #region Фильтр услуг
        List<FeaturesForEventsViewDto> FeaturesList = new List<FeaturesForEventsViewDto>();

        async Task FeaturesChanged(IEnumerable<string> values)
        {
            Filters.SelectedFeatures = values;

            filteredAdmins.Clear();
            filteredRegions.Clear();

            _request.FeaturesIds = FeaturesList
                .Where(x => values.Contains(x.Name))
                .Select(s => s.Id)
                .Distinct();

            await ReloadItemsAsync();
        }

        List<string> _filteredFeatures = new List<string>();
        List<string> filteredFeatures
        {
            get
            {
                if (_filteredFeatures.Count() > 0)
                    return _filteredFeatures;

                IEnumerable<FeaturesForEventsViewDto> linq = FeaturesList;

                if (_request.IsActualEvents)
                    linq = FeaturesList.Where(w => w.EndDate > DateTime.Now);
                else
                    linq = FeaturesList.Where(w => w.EndDate < DateTime.Now);

                if (_request.RegionsIds?.Count() > 0)
                    linq = linq.Where(x => _request.RegionsIds.Contains(x.RegionId));

                if (_request.AdminsIds?.Count() > 0)
                    linq = linq.Where(x => _request.AdminsIds.Contains(x.AdminId));

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

            _request.AdminsIds = AdminsList
                .Where(x => values.Contains(x.Name))
                .Select(s => s.Id)
                .Distinct();

            await ReloadItemsAsync();
        }

        List<string> _filteredAdmins = new List<string>();
        List<string> filteredAdmins
        {
            get
            {
                if (_filteredAdmins.Count() > 0)
                    return _filteredAdmins;

                IEnumerable<AdminsForEventsViewDto> linq = AdminsList;

                if (_request.IsActualEvents)
                    linq = AdminsList.Where(w => w.EndDate > DateTime.Now);
                else
                    linq = AdminsList.Where(w => w.EndDate < DateTime.Now);

                if (_request.RegionsIds?.Count() > 0)
                    linq = linq.Where(x => _request.RegionsIds.Contains(x.RegionId));

                if (_request.FeaturesIds?.Count() > 0)
                    linq = linq.Where(x => _request.FeaturesIds.Contains(x.FeatureId));

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

            _request.RegionsIds = RegionsList
                .Where(x => values.Contains(x.Name))
                .Select(s => s.Id)
                .Distinct();

            await ReloadItemsAsync();
        }

        List<string> _filteredRegions = new List<string>();
        List<string> filteredRegions
        {
            get
            {
                if (_filteredRegions.Count() > 0)
                    return _filteredRegions;

                IEnumerable<RegionsForEventsViewDto> linq = RegionsList;

                if (_request.IsActualEvents)
                    linq = RegionsList.Where(w => w.EndDate > DateTime.Now);
                else
                    linq = RegionsList.Where(w => w.EndDate < DateTime.Now);

                if (_request.AdminsIds?.Count() > 0)
                    linq = linq.Where(x => _request.AdminsIds.Contains(x.AdminId));

                if (_request.FeaturesIds?.Count() > 0)
                    linq = linq.Where(x => _request.FeaturesIds.Contains(x.FeatureId));

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

        async Task ActualEventsChanged(bool value)
        {
            _request.IsActualEvents = value;
            actualEventsLabel = value ? "Актуальные мероприятия" : "Завершённые мероприятия";

            filteredFeatures.Clear();
            Filters.SelectedFeatures = null;
            _request.FeaturesIds = null;

            filteredAdmins.Clear();
            Filters.SelectedAdmins = null;
            _request.AdminsIds = null;

            filteredRegions.Clear();
            Filters.SelectedRegions = null;
            _request.RegionsIds = null;

            await ReloadItemsAsync();
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
