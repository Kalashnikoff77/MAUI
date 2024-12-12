namespace Common.Dto
{
    public class FeaturesForSchedulesDto : DtoBase
    {
        public int EventId { get; set; }

        public Guid Guid { get; set; }

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int? RegionId { get; set; }

        public string? Address { get; set; }

        public short? MaxMen { get; set; }
        public short? MaxWomen { get; set; }
        public short? MaxPairs { get; set; }

        public int? CostMan { get; set; }
        public int? CostWoman { get; set; }
        public int? CostPair { get; set; }

        public bool IsDeleted { get; set; }
    }
}
