using AutoMapper;
using Common.Dto.Requests;
using Common.Dto.Responses;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using System.Data.Common;

namespace WebAPI.Models
{
    public class UnitOfWork : IAsyncDisposable
    {
        public UnitOfWork(string connectionString, IMapper mapper, IMemoryCache cache)
        {
            SqlConnection = new SqlConnection(connectionString);
            Mapper = mapper;
            Cache = cache;
        }

        public DbConnection SqlConnection { get; set; } = null!;
        public DbTransaction? SqlTransaction { get; set; }

        public IMapper Mapper { get; set; }
        public IMemoryCache Cache { get; set; }

        public int? AccountId { get; set; }
        public Guid? AccountGuid { get; set; }

        public async Task BeginTransactionAsync()
        {
            await SqlConnection.OpenAsync();
            SqlTransaction = await SqlConnection.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (SqlTransaction != null)
                await SqlTransaction.CommitAsync();
        }

        public TResponse? CacheTryGet<TRequest, TResponse>(TRequest request, TResponse response, string? prefix = null) where TRequest : RequestDtoBase where TResponse : ResponseDtoBase
        {
            Cache.TryGetValue(request.GetCacheKey(request, prefix), out TResponse? data);
            return data;
        }

        /// <summary>
        /// Установка кэша
        /// </summary>
        /// <param name="key">Request для формирования ключа</param>
        /// <param name="data">Данные для занесения в кэш</param>
        /// <param name="prefix">Префикс для категорий кэша</param>
        /// <param name="expiration">Время хранения кэша в памяти в минутах</param>
        public void CacheSet<TRequest, TResponse>(TRequest key, TResponse data, string? prefix = null, double expiration = 30) where TRequest : RequestDtoBase where TResponse : ResponseDtoBase
        {
            var cacheKey = key.GetCacheKey(key, prefix);
            Cache.Set(cacheKey, data, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(expiration)));
        }

        /// <summary>
        /// Очистить весь кэш
        /// </summary>
        public void CacheClear(string? prefix = null)
        {
            if (prefix != null)
            {
                var keys = ((MemoryCache)Cache).Keys;
                foreach (var key in keys.Where(x => x.ToString()!.StartsWith(prefix + "_")))
                    ((MemoryCache)Cache).Remove(key);
            }
            else
                ((MemoryCache)Cache).Clear();
        }

        public async ValueTask DisposeAsync()
        {
            if (SqlTransaction != null)
                await SqlTransaction.DisposeAsync();
            if (SqlConnection != null)
                await SqlConnection.DisposeAsync();
        }
    }
}
