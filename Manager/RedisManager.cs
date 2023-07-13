using StackExchange.Redis;

namespace NeoServer.Manager
{
    public class RedisManager{
        private ConnectionMultiplexer mRedisConnection;
        private IDatabase mDB;
        
        public RedisManager()
        {
            mRedisConnection=ConnectionMultiplexer.Connect("localhost");
        }
    
    }
}