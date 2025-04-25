using StackExchange.Redis;

public static class RedisExtension
{
    public static IServiceCollection AddRedisCache(this IServiceCollection collection)
    {   
        collection.AddStackExchangeRedisCache(options => 
            {
                options.Configuration = "localhost:6379";
            });
        
        return collection;    
    }

     public static IServiceCollection AddRedisDb(this IServiceCollection collection)
    {   
        
            collection.AddSingleton<IConnectionMultiplexer>(provider => 
            ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"));
        
        return collection;    
    }
}