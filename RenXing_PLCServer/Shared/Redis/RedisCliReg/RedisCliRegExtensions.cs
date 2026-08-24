using Microsoft.Extensions.DependencyInjection;
using Shared.Redis.IRedisCli;
using Shared.Redis.RedisCliByStaEx;

namespace Shared.Redis.RedisCliReg
{
    public static class RedisCliRegExtensions
    {
        public static IServiceCollection RegisterRedisClient(this IServiceCollection services)
        {
            services.AddTransient<IRedisClient, RedisClientByStaEx>();
            return services;
        }
    }
}