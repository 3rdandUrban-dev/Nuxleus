using System;
using System.Collections.Generic;
using System.Text;
using Memcached.ClientLibrary;
using Xameleon.Configuration;

namespace Xameleon.Memcached
{
    public class Client : MemcachedClient
    {
        public Client(MemcachedClient memcachedClient, AspNetMemcachedConfiguration memcachedConfiguration)
        {

            SockIOPool pool = SockIOPool.GetInstance();
            List<string> serverList = new List<string>();
            foreach (MemcachedServer server in memcachedConfiguration.MemcachedServerCollection)
            {
                serverList.Add(server.IP + ":" + server.Port);
            }
            pool.SetServers(serverList.ToArray());

            if (memcachedConfiguration.UseCompression != null && memcachedConfiguration.UseCompression == "yes")
                memcachedClient.EnableCompression = true;
            else
                memcachedClient.EnableCompression = false;

            MemcachedPoolConfig poolConfig = (MemcachedPoolConfig)memcachedConfiguration.PoolConfig;
            pool.InitConnections = (int)poolConfig.InitConnections;
            pool.MinConnections = (int)poolConfig.MinConnections;
            pool.MaxConnections = (int)poolConfig.MaxConnections;
            pool.SocketConnectTimeout = (int)poolConfig.SocketConnectTimeout;
            pool.SocketTimeout = (int)poolConfig.SocketConnect;
            pool.MaintenanceSleep = (int)poolConfig.MaintenanceSleep;
            pool.Failover = (bool)poolConfig.Failover;
            pool.Nagle = (bool)poolConfig.Nagle;
            pool.Initialize();
        }

    }
}
