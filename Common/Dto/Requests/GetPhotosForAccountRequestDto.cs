namespace Common.Dto.Requests
{
    public class GetPhotosForAccountRequestDto : RequestDtoBase
    {
        public override string Uri => "/Photos/Get";

        public int? AccountId { get; set; }
    }
}
