using AutoMapper;
using System.Text.Json;
using WebAPI.Exceptions;

namespace WebAPI.Mapping.Converters
{
    public class JsonToClassConverter<T> : IValueConverter<string?, T?> where T : class
    {
        public T? Convert(string? source, ResolutionContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(source))
                    return null;

                var result = JsonSerializer.Deserialize<T>(source);
                return result;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
    }
}
