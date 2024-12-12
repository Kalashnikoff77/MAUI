using System.Text.Json;

namespace Common.Dto.Requests
{
    public abstract class RequestDtoBase
    {
        public abstract string Uri { get; }

        public string? Token { get; set; }

        public int Take { get; set; } = 20;
        public int Skip { get; set; } = 0;

        public string? FilterFreeText { get; set; }

        public string GetCacheKey<T>(T request, string? prefix = null)
        {
            var hashCode = JsonSerializer.Serialize(request).GetHashCode().ToString();
            return prefix == null ? hashCode : prefix + "_" + hashCode;
        }
    }
}
