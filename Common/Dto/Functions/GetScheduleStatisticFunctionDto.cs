namespace Common.Dto.Functions
{
    public class GetScheduleStatisticFunctionDto
    {
        public int NumOfDiscussions { get; set; }

        public int NumOfRegAccounts { get; set; }
        public int NumOfRegPairs { get; set; }
        public int NumOfRegMen { get; set; }
        public int NumOfRegWomen { get; set; }

        public int AvgAge { get; set; }
    }
}
